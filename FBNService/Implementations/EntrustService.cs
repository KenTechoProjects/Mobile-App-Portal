using EntrustWrapper.Model;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace FBNServices
{
    public class EntrustService
    {
        private ChannelFactory<IAuthWrapper> factory;
        private IAuthWrapper serviceProxy;
        private Binding binding;
        public EntrustService(string url) {
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IAuthWrapper>(binding, new EndpointAddress(url));
            serviceProxy = factory.CreateChannel();
        }
        /// <summary>
        ///  The Token Authentication Method() method accepts the following parameters in the table below as the input. 
        /// </summary>
        /// <param name="staffId"></param>
        /// <param name="token"></param>
        /// <returns>
        /// String response
        /// </returns>
        public async Task<AuthResponse> ValidateToken(string staffId, string token) {
            AuthRequest request = new AuthRequest() {  CustID = staffId, PassCode = token };
            AuthResponse response = new AuthResponse();
            try
            {
                response = await serviceProxy.AuthMethodAsync(request);
               
            }
            catch (Exception e) {
                response.Authenticated = false;
                response.Message = e.Message;
            }

            return response;
        }
    }
}
