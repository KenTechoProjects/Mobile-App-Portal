﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IFBNServices")]
public interface IAdSoapServices
{
    
    // CODEGEN: Generating message contract since element name username from namespace http://tempuri.org/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ADAuthenticator")]
    ADAuthenticatorResponse ADAuthenticator(ADAuthenticatorRequest request);
    
    // CODEGEN: Generating message contract since element name username from namespace http://tempuri.org/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetGroups")]
    GetGroupsResponse GetGroups(GetGroupsRequest request);
    
    // CODEGEN: Generating message contract since element name username from namespace http://tempuri.org/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ADValidateUser")]
    ADValidateUserResponse ADValidateUser(ADValidateUserRequest request);
    
    // CODEGEN: Generating message contract since element name username from namespace http://tempuri.org/ is not marked nillable
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ADUserDetails")]
    ADUserDetailsResponse ADUserDetails(ADUserDetailsRequest request);
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class ADAuthenticatorRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="ADAuthenticator", Namespace="http://tempuri.org/", Order=0)]
    public ADAuthenticatorRequestBody Body;
    
    public ADAuthenticatorRequest()
    {
    }
    
    public ADAuthenticatorRequest(ADAuthenticatorRequestBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class ADAuthenticatorRequestBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string username;
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
    public string password;
    
    public ADAuthenticatorRequestBody()
    {
    }
    
    public ADAuthenticatorRequestBody(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class ADAuthenticatorResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="ADAuthenticatorResponse", Namespace="http://tempuri.org/", Order=0)]
    public ADAuthenticatorResponseBody Body;
    
    public ADAuthenticatorResponse()
    {
    }
    
    public ADAuthenticatorResponse(ADAuthenticatorResponseBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class ADAuthenticatorResponseBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string ADAuthenticatorResult;
    
    public ADAuthenticatorResponseBody()
    {
    }
    
    public ADAuthenticatorResponseBody(string ADAuthenticatorResult)
    {
        this.ADAuthenticatorResult = ADAuthenticatorResult;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class GetGroupsRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="GetGroups", Namespace="http://tempuri.org/", Order=0)]
    public GetGroupsRequestBody Body;
    
    public GetGroupsRequest()
    {
    }
    
    public GetGroupsRequest(GetGroupsRequestBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class GetGroupsRequestBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string username;
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
    public string password;
    
    public GetGroupsRequestBody()
    {
    }
    
    public GetGroupsRequestBody(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class GetGroupsResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="GetGroupsResponse", Namespace="http://tempuri.org/", Order=0)]
    public GetGroupsResponseBody Body;
    
    public GetGroupsResponse()
    {
    }
    
    public GetGroupsResponse(GetGroupsResponseBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class GetGroupsResponseBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string GetGroupsResult;
    
    public GetGroupsResponseBody()
    {
    }
    
    public GetGroupsResponseBody(string GetGroupsResult)
    {
        this.GetGroupsResult = GetGroupsResult;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class ADValidateUserRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="ADValidateUser", Namespace="http://tempuri.org/", Order=0)]
    public ADValidateUserRequestBody Body;
    
    public ADValidateUserRequest()
    {
    }
    
    public ADValidateUserRequest(ADValidateUserRequestBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class ADValidateUserRequestBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string username;
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
    public string password;
    
    public ADValidateUserRequestBody()
    {
    }
    
    public ADValidateUserRequestBody(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class ADValidateUserResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="ADValidateUserResponse", Namespace="http://tempuri.org/", Order=0)]
    public ADValidateUserResponseBody Body;
    
    public ADValidateUserResponse()
    {
    }
    
    public ADValidateUserResponse(ADValidateUserResponseBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class ADValidateUserResponseBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string ADValidateUserResult;
    
    public ADValidateUserResponseBody()
    {
    }
    
    public ADValidateUserResponseBody(string ADValidateUserResult)
    {
        this.ADValidateUserResult = ADValidateUserResult;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class ADUserDetailsRequest
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="ADUserDetails", Namespace="http://tempuri.org/", Order=0)]
    public ADUserDetailsRequestBody Body;
    
    public ADUserDetailsRequest()
    {
    }
    
    public ADUserDetailsRequest(ADUserDetailsRequestBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class ADUserDetailsRequestBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string username;
    
    public ADUserDetailsRequestBody()
    {
    }
    
    public ADUserDetailsRequestBody(string username)
    {
        this.username = username;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
public partial class ADUserDetailsResponse
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="ADUserDetailsResponse", Namespace="http://tempuri.org/", Order=0)]
    public ADUserDetailsResponseBody Body;
    
    public ADUserDetailsResponse()
    {
    }
    
    public ADUserDetailsResponse(ADUserDetailsResponseBody Body)
    {
        this.Body = Body;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
public partial class ADUserDetailsResponseBody
{
    
    [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
    public string ADUserDetailsResult;
    
    public ADUserDetailsResponseBody()
    {
    }
    
    public ADUserDetailsResponseBody(string ADUserDetailsResult)
    {
        this.ADUserDetailsResult = ADUserDetailsResult;
    }
}
