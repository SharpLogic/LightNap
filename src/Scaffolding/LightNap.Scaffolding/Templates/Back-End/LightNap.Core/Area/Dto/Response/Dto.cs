﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace LightNap.Scaffolding.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Dto\Response\Dto.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class Dto : LightNap.Scaffolding.Templates.BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nnamespace ");
            
            #line 7 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Dto\Response\Dto.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 7 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Dto\Response\Dto.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Response.Dto\r\n{\r\n    public class ");
            
            #line 9 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Dto\Response\Dto.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto\r\n    {\r\n        // TODO: Finalize which fields to include when returning this" +
                    " item.\r\n        public ");
            
            #line 12 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Dto\Response\Dto.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.ServerIdType));
            
            #line default
            #line hidden
            this.Write(" Id { get; set; }\r\n        ");
            
            #line 13 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Dto\Response\Dto.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.ServerPropertiesList));
            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}