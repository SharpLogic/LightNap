﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace LightNap.Core.Email.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using LightNap.Core.Data.Entities;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\LightNap.Core\Email\Templates\RegistrationWelcomeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class RegistrationWelcomeTemplate : BaseTemplate
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n");
            
            #line 8 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\LightNap.Core\Email\Templates\RegistrationWelcomeTemplate.tt"
 base.TransformText(); 
            
            #line default
            #line hidden
            this.Write("\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 10 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\LightNap.Core\Email\Templates\RegistrationWelcomeTemplate.tt"
 
	protected override void RenderBody() 
	{

        
        #line default
        #line hidden
        
        #line 13 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\LightNap.Core\Email\Templates\RegistrationWelcomeTemplate.tt"
this.Write("<p>Welcome to our site! Thank you for registering.</p>\r\n");

        
        #line default
        #line hidden
        
        #line 15 "C:\Users\edkai\source\repos\SharpLogic\LightNap\src\LightNap.Core\Email\Templates\RegistrationWelcomeTemplate.tt"

	}

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}