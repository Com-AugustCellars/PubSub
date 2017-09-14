using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.AugustCellars.CoAP;
using Com.AugustCellars.CoAP.Server.Resources;

namespace Com.AugustCellars.CoAP.PubSub
{
    /// <summary>
    /// Class implementing the leaf behavior for the PubSub REST API
    /// </summary>
    public class PubSubLeaf : Resource
    {
        private byte[] _content;
        private int _contentType;
        private DateTime _publishTime;

        /// <summary>
        /// Time in seconds from the publish to expiration
        /// 0 means never expire.
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// Construct a PubSubLeaf node
        /// </summary>
        /// <param name="name">name of the node</param>
        public PubSubLeaf(string name) : base(name)
        {
            Observable = true;
            _publishTime = DateTime.Now;
        }

        /// <inheritdoc />
        protected override void DoDelete(CoapExchange exchange)
        {
            Parent.Remove(this);
            exchange.Respond(StatusCode.Deleted);
        }

        /// <inheritdoc />
        protected override void DoGet(CoapExchange exchange)
        {
            if (IsExpired()) {
                exchange.Respond(StatusCode.NotFound);
                return;
            }

            Request request = exchange.Request;
            byte[] payload = _content;
            Response response = Response.CreateResponse(request, StatusCode.Content);

            //  Can I convert from format to desired format?
            if (request.HasOption(OptionType.Accept)) {
                foreach (Option option in request.GetOptions(OptionType.Accept)) {
                    try {
                        payload = ConvertTo(option.IntValue);
                        break;
                    }
                    catch {
                    }
                    ;
                }
            }

            if (MaxAge > 0) exchange.MaxAge = MaxAge;
            response.ContentFormat = _contentType;
            response.Payload = payload;

            exchange.Respond(response);
        }

        /// <inheritdoc />
        protected override void DoPost(CoapExchange exchange)
        {
            base.DoPost(exchange);
        }

        /// <inheritdoc />
        protected override void DoPut(CoapExchange exchange)
        {
            Request req = exchange.Request;
            if (!req.HasOption(OptionType.ContentType)) {
                exchange.Respond(StatusCode.BadOption);
                return;
            }

            string contentType = req.GetFirstOption(OptionType.ContentType).StringValue;
            bool found = Attributes.GetContentTypes().Any(legal => legal == contentType);

            if (!found) {
                exchange.Respond(StatusCode.BadOption);
                return;
            }


            if (contentType != _contentType.ToString() || !req.Payload.SequenceEqual(_content)) {

                _contentType = int.Parse(contentType);
                _content = req.Payload;

                Changed();
            }

            if (req.HasOption(OptionType.MaxAge)) {
                MaxAge = req.GetFirstOption(OptionType.MaxAge).IntValue;
            }

            _publishTime = DateTime.Now;

            exchange.Respond(StatusCode.Changed);
        }


        /// <summary>
        /// Change content type between the acutal content and the desired content.
        /// Default action is to never do conversions
        /// </summary>
        /// <param name="accept">Media Type that is desired</param>
        /// <returns>converted content</returns>
        public virtual byte[] ConvertTo(int accept)
        {
            if (accept != _contentType) {
                throw new Exception("Can't convert");
            }

            return _content;
        }

        /// <summary>
        /// Is the content in this leaf expired?
        /// </summary>
        /// <returns>true if expired</returns>
        public bool IsExpired()
        {
            if (MaxAge == 0) {
                return false;
            }

            if (DateTime.Now > _publishTime + TimeSpan.FromSeconds(MaxAge)) {
                return true;
            }
            return false;
        }
    }
}
