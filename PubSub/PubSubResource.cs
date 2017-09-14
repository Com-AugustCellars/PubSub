using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.AugustCellars.CoAP;
using Com.AugustCellars.CoAP.Server.Resources;

namespace PubSub
{
    public class PubSubResource : Resource
    {
        private readonly bool _doDiscovery;

        public PubSubResource(string name, bool doDiscovery = true) : base(name)
        {
            Attributes.AddResourceType("core.ps");
            _doDiscovery = doDiscovery;
            if (doDiscovery) {
                Attributes.AddResourceType("core.ps.discover");
            }
            Attributes.AddContentType(MediaType.ApplicationLinkFormat);
            Attributes.AddContentType(64);
        }

        /// <summary>
        /// A Get does the discovery thing on this node and it's children.
        /// </summary>
        /// <param name="exchange">information about current conversation</param>
        protected override void DoGet(CoapExchange exchange)
        {
            Request request = exchange.Request;

            if (!_doDiscovery) {
                exchange.Respond(StatusCode.MethodNotAllowed);
                return;
            }

            if (request.HasOption(OptionType.Accept)) {
                IEnumerable<Option> options = request.GetOptions(OptionType.Accept);

                foreach (Option opt in options) {
                    switch (opt.IntValue) {
                    case MediaType.ApplicationLinkFormat:
                        exchange.Respond(StatusCode.Content,
                            LinkFormat.Serialize(this, exchange.Request.UriQueries),
                            MediaType.ApplicationLinkFormat);
                        return;

                    case 64: // application/link-format+cbor
                        exchange.Respond(StatusCode.Content, 
                            LinkFormat.SerializeCbor(this, exchange.Request.UriQueries), 64);
                        return;

                    default:
                        //  Unsupported format
                        break;
                    }
                }

                //  None of the requested formats that were asked for are supported.
                //  Return the default format.
            }

            exchange.Respond(StatusCode.Content,
                LinkFormat.Serialize(this, exchange.Request.UriQueries),
                MediaType.ApplicationLinkFormat);
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        /// <param name="exchange">Information on current conversation</param>
        protected override void DoPost(CoapExchange exchange)
        {
            Request request = exchange.Request;
            WebLink linkInfo;

            //  Parse the content based on the content type.
            //  Assume content-type of =40

            if (request.HasOption(OptionType.ContentFormat)) {
                switch (request.ContentFormat) {
                case MediaType.ApplicationLinkFormat:
                    linkInfo = LinkFormat.Parse(request.PayloadString).First();
                    break;

                case 64:
                    linkInfo = LinkFormat.Parse(request.PayloadString).First();
                    break;

                default:
                    exchange.Respond(StatusCode.BadRequest);
                    return;
                }
            }
            else {
                linkInfo = LinkFormat.Parse(request.PayloadString).First();
            }

            //  Is this a single level create?

            //  Create the topic

            IResource child;
            if (linkInfo.Attributes.GetContentTypes().Any(p =>
                p.Equals("40")
            )) {
                child = new PubSubResource(linkInfo.Uri);
            }
            else {
                child = new Resource(linkInfo.Uri);
                foreach (string key in linkInfo.Attributes.Keys) {
                    bool f = true;
                    foreach (string value in linkInfo.Attributes.GetValues(key)) {
                        child.Attributes.Add(key, value);
                        f = false;
                    }
                    if (f) child.Attributes.Add(key);
                }
            }

            this.Add(child);

            exchange.LocationPath = child.Uri;
            exchange.Respond(StatusCode.Created);
            

        }
    }
}
