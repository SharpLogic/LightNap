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
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DataService : LightNap.Scaffolding.Templates.BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nimport { HttpClient } from \"@angular/common/http\";\r\nimport { Injectable, inject" +
                    " } from \"@angular/core\";\r\nimport { API_URL_ROOT, ApiResponse, PagedResponse } fr" +
                    "om \"@core\";\r\nimport { tap } from \"rxjs\";\r\nimport {");
            
            #line 11 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Helper } from \"../helpers/");
            
            #line 11 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabName));
            
            #line default
            #line hidden
            this.Write(".helper\";\r\nimport { Create");
            
            #line 12 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Request } from \"../models/request/create-");
            
            #line 12 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabName));
            
            #line default
            #line hidden
            this.Write("-request\";\r\nimport { Search");
            
            #line 13 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("Request } from \"../models/request/search-");
            
            #line 13 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabNamePlural));
            
            #line default
            #line hidden
            this.Write("-request\";\r\nimport { Update");
            
            #line 14 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Request } from \"../models/request/update-");
            
            #line 14 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabName));
            
            #line default
            #line hidden
            this.Write("-request\";\r\nimport { ");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(" } from \"../models/response/");
            
            #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.KebabName));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n@Injectable({\r\n  providedIn: \"root\",\r\n})\r\nexport class DataService {\r\n  #ht" +
                    "tp = inject(HttpClient);\r\n  #apiUrlRoot = `${inject(API_URL_ROOT)}");
            
            #line 22 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("/`;\r\n\r\n  get");
            
            #line 24 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 24 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(": ");
            
            #line 24 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.FrontEndType));
            
            #line default
            #line hidden
            this.Write(") {\r\n    return this.#http.get<ApiResponse<");
            
            #line 25 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(">>(`${this.#apiUrlRoot}${");
            
            #line 25 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("}`).pipe(\r\n      tap(response => {\r\n        if (response.result) {\r\n          ");
            
            #line 28 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Helper.rehydrate(response.result);\r\n        }\r\n      })\r\n    );\r\n  }\r\n\r\n  search");
            
            #line 34 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("(request: Search");
            
            #line 34 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("Request) {\r\n    return this.#http.post<ApiResponse<PagedResponse<");
            
            #line 35 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(">>>(`${this.#apiUrlRoot}search`, request).pipe(\r\n      tap(response => {\r\n       " +
                    " if (response.result) {\r\n          response.result.data.forEach(");
            
            #line 38 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Helper.rehydrate);\r\n        }\r\n      })\r\n    );\r\n  }\r\n\r\n  create");
            
            #line 44 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(request: Create");
            
            #line 44 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Request) {\r\n    return this.#http.post<ApiResponse<");
            
            #line 45 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(">>(`${this.#apiUrlRoot}`, request);\r\n  }\r\n\r\n  update");
            
            #line 48 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 48 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(": ");
            
            #line 48 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.FrontEndType));
            
            #line default
            #line hidden
            this.Write(", request: Update");
            
            #line 48 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Request) {\r\n    return this.#http.put<ApiResponse<");
            
            #line 49 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write(">>(`${this.#apiUrlRoot}${");
            
            #line 49 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("}`, request);\r\n  }\r\n\r\n  delete");
            
            #line 52 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 52 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(": ");
            
            #line 52 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.FrontEndType));
            
            #line default
            #line hidden
            this.Write(") {\r\n    return this.#http.delete<ApiResponse<boolean>>(`${this.#apiUrlRoot}${");
            
            #line 53 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Front-End\Area\Services\DataService.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("}`);\r\n  }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
