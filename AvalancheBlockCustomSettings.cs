using System.Collections.Generic;
using System.Web;
using Avalanche.Models;
using Rock;
using Rock.Attribute;
using Rock.Web.UI;

namespace Avalanche
{
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public abstract class AvalancheBlockCustomSettings : RockBlockCustomSettings, IMobileResource
    {
        private Dictionary<string, string> _customAtributes;
        public Dictionary<string, string> CustomAttributes
        {
            get
            {
                if ( _customAtributes == null )
                {
                    _customAtributes = new Dictionary<string, string>();
                    var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
                    foreach ( var item in customs )
                    {
                        _customAtributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
                    }
                }
                return _customAtributes;
            }
        }
        public abstract MobileBlock GetMobile( string arg );
        public virtual MobileBlockResponse HandleRequest( string resource, Dictionary<string, string> Body )
        {
            return new MobileBlockResponse()
            {
                Arg = resource,
                Response = "",
                TTL = 0
            };
        }
    }
}