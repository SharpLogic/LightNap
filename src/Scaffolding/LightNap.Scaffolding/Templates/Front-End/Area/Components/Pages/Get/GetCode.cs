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
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class GetCode : LightNap.Scaffolding.Templates.BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"
import { CommonModule } from ""@angular/common"";
import { Component, inject, input, OnInit } from ""@angular/core"";
import { RouterLink } from ""@angular/router"";
import { ApiResponse, ApiResponseComponent } from ""@core"";
import { ButtonModule } from ""primeng/button"";
import { CardModule } from ""primeng/card"";
import { Observable } from ""rxjs"";
import { ");
            
            #line 14 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(" } from \"src/app/");
            
            #line 14 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabNamePlural));
            
            #line default
            #line hidden
            this.Write("/models/response/");
            
            #line 14 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabName));
            
            #line default
            #line hidden
            this.Write("\";\r\nimport { ");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Service } from \"src/app/");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabNamePlural));
            
            #line default
            #line hidden
            this.Write("/services/");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabName));
            
            #line default
            #line hidden
            this.Write(".service\";\r\n\r\n@Component({\r\n  standalone: true,\r\n  templateUrl: \"./get.component." +
                    "html\",\r\n  imports: [CommonModule, CardModule, RouterLink, ApiResponseComponent, " +
                    "ButtonModule],\r\n})\r\nexport class GetComponent implements OnInit {\r\n  #");
            
            #line 23 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelName));
            
            #line default
            #line hidden
            this.Write("Service = inject(");
            
            #line 23 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Service);\r\n  errors = new Array<string>();\r\n\r\n  readonly id = input<number>(undef" +
                    "ined);\r\n  ");
            
            #line 27 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelName));
            
            #line default
            #line hidden
            this.Write("$?: Observable<ApiResponse<");
            
            #line 27 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(">>;\r\n\r\n  ngOnInit() {\r\n    this.");
            
            #line 30 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelName));
            
            #line default
            #line hidden
            this.Write("$ = this.#");
            
            #line 30 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelName));
            
            #line default
            #line hidden
            this.Write("Service.get");
            
            #line 30 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Components\Pages\Get\GetCode.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(this.id());\r\n  }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
