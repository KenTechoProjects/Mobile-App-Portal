using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FBNServices
{
    public class ADService
    {
        private ChannelFactory<IAdSoapServices> factory;
        private IAdSoapServices serviceProxy;
        private Binding binding;
        public ADService(string url) {
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IAdSoapServices>(binding, new EndpointAddress(url));
            serviceProxy = factory.CreateChannel();
        }
        /// <summary>
        ///  The ADAuthenticator() method accepts the following parameters in the table below as the input. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>
        /// String response
        /// </returns>
        public string ADAuthenticator(string username, string password) {
            ADAuthenticatorRequestBody body = new ADAuthenticatorRequestBody(username, password);
            ADAuthenticatorRequest request = new ADAuthenticatorRequest(body);
            try
            {
                var result = serviceProxy.ADAuthenticator(request);
                return  result.Body.ADAuthenticatorResult.ToString();
            }
            catch (Exception e) {
      
                return "Something Wrong";
            }
            
        }

        /// <summary>
        /// The GetGroups() method accepts the following parameters in the table below as the input. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns> string reponse</returns>

        public string GetGroups(string username ,string password) {
            GetGroupsRequestBody requestBody = new GetGroupsRequestBody(username, password);
            GetGroupsRequest getGroupsRequest = new GetGroupsRequest(requestBody);
            try
            {
                var result = serviceProxy.GetGroups(getGroupsRequest);
                return result.Body.GetGroupsResult.ToString();
            }
            catch (Exception e)
            {

                return "Something Wrong";
            }
        }


        /// <summary>
        /// The ADValidateUser() method accepts the following parameters in the table below as the input.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string ADValidateUser(string username, string password) {

            ADValidateUserRequestBody  body= new ADValidateUserRequestBody(username, password);
            ADValidateUserRequest request = new ADValidateUserRequest(body);
            try
            {
                var result = serviceProxy.ADValidateUser(request);
                return result.Body.ADValidateUserResult.ToString();
            }
            catch (Exception e)
            {
                return "Something Wrong";
            }
        }

        /// <summary>
        /// 
        ///The ADUserDetails() method accepts the following parameters in the table below as the input. 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string ADUserDetails(string username) {

            ADUserDetailsRequestBody body = new ADUserDetailsRequestBody(username);
            ADUserDetailsRequest request = new ADUserDetailsRequest(body);
            try
            {
                var result = serviceProxy.ADUserDetails(request);

                return result.Body.ADUserDetailsResult.ToString();
            }
            catch (Exception e)
            {
                return "Something Wrong";
            }
        }


    }
}
