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
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class CreateHtml : LightNap.Scaffolding.Templates.BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"
<p-card header=""Create"">
  <div class=""flex gap-1 mb-1"">
    <p-button [routerLink]=""['..']"" icon=""pi pi-arrow-up"" label=""See all"" />
  </div>

  <form [formGroup]=""form"" (ngSubmit)=""createClicked()"" autocomplete=""off"">
    <div class=""w-30rem flex flex-column gap-4"">
");
            
            #line 14 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
 if (Parameters.IdProperty.IsRequired) { 
            
            #line default
            #line hidden
            this.Write("          <div class=\"w-16rem\">\r\n");
            
            #line 16 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
      if (Parameters.IdProperty.FrontEndType == "number") { 
            
            #line default
            #line hidden
            this.Write("              <label for=\"");
            
            #line 17 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"block text-900 font-medium mb-2\">");
            
            #line 17 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.Name));
            
            #line default
            #line hidden
            this.Write("</label>\r\n              <p-inputNumber id=\"");
            
            #line 18 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 18 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("\" styleClass=\"w-full\" />\r\n");
            
            #line 19 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
	    } else { 
            
            #line default
            #line hidden
            this.Write("              <label for=\"");
            
            #line 20 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"block text-900 font-medium mb-2\">");
            
            #line 20 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.Name));
            
            #line default
            #line hidden
            this.Write("</label>\r\n              <input pInputText id=\"");
            
            #line 21 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 21 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"w-full\" />\r\n");
            
            #line 22 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
 }
            
            #line default
            #line hidden
            this.Write("          </div>\r\n");
            
            #line 24 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
 } 
            
            #line default
            #line hidden
            
            #line 25 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
 foreach (var property in Parameters.SetProperties) { 
            
            #line default
            #line hidden
            this.Write("      <div>\r\n");
            
            #line 27 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
      switch (property.FrontEndType) {
            case "boolean": 
            
            #line default
            #line hidden
            this.Write("          <p-checkbox label=\"");
            
            #line 29 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 29 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" [binary]=\"true\" class=\"w-full\" />\r\n");
            
            #line 30 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
              break;
            case "Date": 
            
            #line default
            #line hidden
            this.Write("              <label for=\"");
            
            #line 32 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"block text-900 font-medium mb-2\">");
            
            #line 32 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("</label>\r\n              <p-calendar id=\"");
            
            #line 33 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 33 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" [showTime]=\"true\" [showIcon]=\"true\" />\r\n");
            
            #line 34 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
              break;            
            case "number": 
            
            #line default
            #line hidden
            this.Write("          <label for=\"");
            
            #line 36 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"block text-900 font-medium mb-2\">");
            
            #line 36 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("</label>\r\n");
            
            #line 37 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
              switch(property.BackEndType) {
				    case "double":
				    case "float":
				    case "decimal":

            
            #line default
            #line hidden
            this.Write("          <p-inputNumber id=\"");
            
            #line 42 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 42 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" styleClass=\"w-full\" mode=\"decimal\" [minFractionDigits]=\"2\" />\r\n");
            
            #line 43 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
                      break;
                    default: 
            
            #line default
            #line hidden
            this.Write("          <p-inputNumber id=\"");
            
            #line 45 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 45 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" styleClass=\"w-full\" />\r\n");
            
            #line 46 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
                  break;
			    }
                break;
            default:
            
            #line default
            #line hidden
            this.Write("          <label for=\"");
            
            #line 50 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"block text-900 font-medium mb-2\">");
            
            #line 50 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write("</label>\r\n          <input pInputText id=\"");
            
            #line 51 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" formControlName=\"");
            
            #line 51 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.CamelName));
            
            #line default
            #line hidden
            this.Write("\" class=\"w-full\" />\r\n");
            
            #line 52 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
          break;      
        }
            
            #line default
            #line hidden
            this.Write("      </div>\r\n");
            
            #line 55 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Create\CreateHtml.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n      <error-list [errors]=\"errors\" />\r\n      <p-button type=\"submit\" label=\"Cr" +
                    "eate\" icon=\"pi pi-save\" severity=\"success\" [disabled]=\"!form.valid\" />\r\n    </di" +
                    "v>\r\n  </form>\r\n</p-card>\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
