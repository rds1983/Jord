﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wanderers.Generator {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Wanderers.Generator.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&quot;map&quot;: {
        ///	&quot;id&quot;: &quot;world&quot;,
        ///	&quot;local&quot;: &quot;false&quot;,
        ///	&quot;legend&quot;: {
        ///		&quot;$&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;buildingWall&quot;
        ///		},
        ///		&quot;#&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;cityWall&quot;
        ///		},
        ///		&quot;1&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;floor&quot;
        ///		},
        ///		&quot;.&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;grass&quot;
        ///		},
        ///		&quot;0&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;ground&quot;
        ///		},
        ///		&quot;/&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;highGrass&quot;
        ///		},
        ///		&quot;h&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;hill&quot;
        ///		},
        ///		&quot;M&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;mountain&quot;
        ///		},
        ///		&quot;T&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;tree&quot;
        ///		},
        ///		&quot;w&quot;: {
        ///			&quot;tileInfoId&quot;: &quot;water&quot;
        ///		}
        ///	},
        ///	&quot;data&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Template {
            get {
                return ResourceManager.GetString("Template", resourceCulture);
            }
        }
    }
}
