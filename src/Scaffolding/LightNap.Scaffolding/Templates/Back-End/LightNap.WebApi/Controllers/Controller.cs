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
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class Controller : LightNap.Scaffolding.Templates.BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nusing ");
            
            #line 7 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".Api;\r\nusing ");
            
            #line 8 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 8 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Interfaces;\r\nusing ");
            
            #line 9 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 9 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Dto.Request;\r\nusing ");
            
            #line 10 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CoreNamespace));
            
            #line default
            #line hidden
            this.Write(".");
            
            #line 10 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.NameForNamespace));
            
            #line default
            #line hidden
            this.Write(".Dto.Response;\r\nusing Microsoft.AspNetCore.Mvc;\r\n\r\nnamespace ");
            
            #line 13 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.WebApiNamespace));
            
            #line default
            #line hidden
            this.Write(@".Controllers
{
    // TODO: Update authorization for methods via the Authorize attribute at the controller or method level.
    // Also register this controller's dependencies in the AddApplicationServices method of Extensions/ApplicationServiceExtensions.cs:
    //
    // services.AddScoped<I");
            
            #line 18 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Service, ");
            
            #line 18 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Service>();\r\n    //\r\n    [ApiController]\r\n    [Route(\"api/[controller]\")]\r\n    pu" +
                    "blic class ");
            
            #line 22 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("Controller(I");
            
            #line 22 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Service ");
            
            #line 22 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelNamePlural));
            
            #line default
            #line hidden
            this.Write("Service) : ControllerBase\r\n    {\r\n        [HttpGet(\"{");
            
            #line 24 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("}\")]\r\n        [ProducesResponseType(typeof(ApiResponseDto<");
            
            #line 25 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>), 200)]\r\n        public async Task<ApiResponseDto<");
            
            #line 26 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>> Get");
            
            #line 26 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 26 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.BackEndType));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 26 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n            return new ApiResponseDto<");
            
            #line 28 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>(await ");
            
            #line 28 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelNamePlural));
            
            #line default
            #line hidden
            this.Write("Service.Get");
            
            #line 28 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Async(");
            
            #line 28 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("));\r\n        }\r\n\r\n        [HttpPost(\"search\")]\r\n        [ProducesResponseType(typ" +
                    "eof(ApiResponseDto<PagedResponseDto<");
            
            #line 32 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>>), 200)]\r\n        public async Task<ApiResponseDto<PagedResponseDto<");
            
            #line 33 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>>> Search");
            
            #line 33 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("([FromBody] Search");
            
            #line 33 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("Dto dto)\r\n        {\r\n            return new ApiResponseDto<PagedResponseDto<");
            
            #line 35 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>>(await ");
            
            #line 35 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelNamePlural));
            
            #line default
            #line hidden
            this.Write("Service.Search");
            
            #line 35 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalNamePlural));
            
            #line default
            #line hidden
            this.Write("Async(dto));\r\n        }\r\n\r\n        [HttpPost]\r\n        [ProducesResponseType(type" +
                    "of(ApiResponseDto<");
            
            #line 39 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>), 201)]\r\n        public async Task<ApiResponseDto<");
            
            #line 40 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>> Create");
            
            #line 40 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("([FromBody] Create");
            
            #line 40 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto dto)\r\n        {\r\n            return new ApiResponseDto<");
            
            #line 42 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>(await ");
            
            #line 42 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelNamePlural));
            
            #line default
            #line hidden
            this.Write("Service.Create");
            
            #line 42 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Async(dto));\r\n        }\r\n\r\n        [HttpPut(\"{");
            
            #line 45 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("}\")]\r\n        [ProducesResponseType(typeof(ApiResponseDto<");
            
            #line 46 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>), 200)]\r\n        public async Task<ApiResponseDto<");
            
            #line 47 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>> Update");
            
            #line 47 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 47 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.BackEndType));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 47 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(", [FromBody] Update");
            
            #line 47 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto dto)\r\n        {\r\n            return new ApiResponseDto<");
            
            #line 49 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Dto>(await ");
            
            #line 49 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelNamePlural));
            
            #line default
            #line hidden
            this.Write("Service.Update");
            
            #line 49 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Async(");
            
            #line 49 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(", dto));\r\n        }\r\n\r\n        [HttpDelete(\"{");
            
            #line 52 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write("}\")]\r\n        [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]\r\n        " +
                    "public async Task<ApiResponseDto<bool>> Delete");
            
            #line 54 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 54 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.BackEndType));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 54 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(")\r\n        {\r\n            await ");
            
            #line 56 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.CamelNamePlural));
            
            #line default
            #line hidden
            this.Write("Service.Delete");
            
            #line 56 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.PascalName));
            
            #line default
            #line hidden
            this.Write("Async(");
            
            #line 56 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\Scaffolding\LightNap.Scaffolding\Templates\Back-End\LightNap.WebApi\Controllers\Controller.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Parameters.IdProperty.CamelName));
            
            #line default
            #line hidden
            this.Write(");\r\n            return new ApiResponseDto<bool>(true);\r\n        }\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
