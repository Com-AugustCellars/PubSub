using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.AugustCellars.CoAP;
using Com.AugustCellars.CoAP.Server.Resources;

namespace PubSub
{
    public class PubSubLeaf : Resource
    {
        private byte[] _content;
        private int _contentType;
        private int _maxAge = 0;

        public PubSubLeaf(string name) : base(name)
        {
            Observable = true;
        }

        protected override void DoDelete(CoapExchange exchange)
        {
            base.DoDelete(exchange);
        }

        protected override void DoGet(CoapExchange exchange)
        {
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
                    catch { }
                    ;
                }
            }

            if (_maxAge > 0) exchange.MaxAge = _maxAge;
            response.ContentFormat = _contentType;
            response.Payload = payload;

            exchange.Respond(response);
        }

        protected override void DoPost(CoapExchange exchange)
        {
            base.DoPost(exchange);
        }

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

            exchange.Respond(StatusCode.Changed);
        }


        public virtual byte[] ConvertTo(int accept)
        {
            if (accept != _contentType) {
                throw new Exception("Can't convert");
            }

            return _content;
        }
    }
}
