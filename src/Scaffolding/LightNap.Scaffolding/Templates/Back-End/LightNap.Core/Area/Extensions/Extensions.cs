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
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class Extensions : LightNap.Scaffolding.Templates.BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nusing ");
            
            #line 7 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".Data.Entities;\r\nusing ");
            
            #line 8 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 8 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Request.Dto;\r\nusing ");
            
            #line 9 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 9 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Response.Dto;\r\n\r\nnamespace ");
            
            #line 11 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 11 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Extensions\r\n{\r\n    public static class ");
            
            #line 13 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Extensions\r\n    {\r\n        public static ");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(" ToCreate(this Create");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto dto)\r\n        {\r\n            // TODO: Update these fields to match the DTO.\r\n" +
                    "            var item = new ");
            
            #line 18 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("();\r\n");
            
            #line 19 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
 foreach (var property in Parameters.SetProperties) { 
            
            #line default
            #line hidden
            this.Write("            item.");
            
            #line 20 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(" = dto.");
            
            #line 20 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 21 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            return item;\r\n        }\r\n\r\n        public static ");
            
            #line 25 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto ToDto(this ");
            
            #line 25 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(" item)\r\n        {\r\n            // TODO: Update these fields to match the DTO.\r\n  " +
                    "          var dto = new ");
            
            #line 28 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto();\r\n            dto.");
            
            #line 29 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.Name));
            
            #line default
            #line hidden
            this.Write(" = item.");
            
            #line 29 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 30 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
 foreach (var property in Parameters.GetProperties) { 
            
            #line default
            #line hidden
            this.Write("            dto.");
            
            #line 31 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(" = item.");
            
            #line 31 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 32 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            return dto;\r\n        }\r\n\r\n        public static void UpdateFromDto(th" +
                    "is ");
            
            #line 36 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(" item, Update");
            
            #line 36 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto dto)\r\n        {\r\n            // TODO: Update these fields to match the DTO.\r\n" +
                    "");
            
            #line 39 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
 foreach (var property in Parameters.SetProperties) { 
            
            #line default
            #line hidden
            this.Write("            item.");
            
            #line 40 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(" = dto.");
            
            #line 40 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(";\r\n");
            
            #line 41 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.Core\Area\Extensions\Extensions.tt"
 } 
            
            #line default
            #line hidden
            this.Write("        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
