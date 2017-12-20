using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Attribute;

namespace Avalanche.Attribute
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public class ActionItemFieldAttribute : FieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckinGroupFieldAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="defaultGroupGuid">The default group unique identifier.</param>
        /// <param name="category">The category.</param>
        /// <param name="order">The order.</param>
        /// <param name="key">The key.</param>
        public ActionItemFieldAttribute( string name, string description = "", bool required = true, string defaultGroupGuid = "", string category = "", int order = 0, string key = null )
            : base( name, description, required, defaultGroupGuid, category, order, key, typeof( Avalanche.Field.Types.ActionItemFieldType ).FullName, "Avalanche" )
        {
        }
    }
}
