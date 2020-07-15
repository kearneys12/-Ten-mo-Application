using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AuthService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
        bool addFundsResult = false;

        //login endpoints
        public bool Register(LoginUser registerUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login/register");
            request.AddJsonBody(registerUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    throw new Exception("An error message was received: " + response.Data.Message);
                }
                else
                {
                    throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
            }
            else
            {
                return true;
            }
        }

        public API_User Login(LoginUser loginUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login");
            request.AddJsonBody(loginUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    throw new Exception("An error message was received: " + response.Data.Message);
                }
                else
                {
                    throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }

            }
            else
            {
                client.Authenticator = new JwtAuthenticator(response.Data.Token);
                return response.Data;
            }
        }
        public int GetBalance(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transaction/"+ userId);
            IRestResponse<int> response = client.Get<int>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {
               
                    throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
            
            }
            else
            {
                return response.Data;
            }
        }

        public string GetUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transaction/users");
            IRestResponse<List<string>> response = client.Get<List<string>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {

                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);

            }
            else
            {
                string userList = "";
                foreach (string user in response.Data)
                {
                    userList += user + Environment.NewLine;
                    
                }
                return userList;
            }

        }

        public bool AddFunds(decimal transferAmount, int transferRecipientID)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"transaction/add/{transferRecipientID}/{transferAmount}");
            IRestResponse<bool> response = client.Put<bool>(request);

            try
            {
                if (response.ResponseStatus != ResponseStatus.Completed)
                {
                    throw new Exception("An error occurred communicating with the server.");

                }
                else if (!response.IsSuccessful)
                {

                    throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);

                }
                else
                {
                    addFundsResult = true;
                    return response.Data;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public bool TransferFunds(decimal transferAmount, int userId, int transferRecipientId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"transaction/deduct/{userId}/{transferAmount}/{transferRecipientId}");
            IRestResponse<bool> response = client.Put<bool>(request);

            if (GetBalance(userId) < transferAmount || addFundsResult == false)
            {
                return false;
            }
            else
            {
                try
                {
                    if (response.ResponseStatus != ResponseStatus.Completed)
                    {
                        throw new Exception("An error occurred communicating with the server.");

                    }
                    else if (!response.IsSuccessful)
                    {

                        throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);

                    }
                    else
                    {

                        return response.Data;
                    }

                }
                catch
                {
                    return false;
                }
            }           
        }
        public List<string> GetAllTransfers(int account)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transaction/transfers/" + account);
            IRestResponse<List<string>> response = client.Get<List<string>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {

                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);

            }
            else
            {
                return response.Data;
            }
        }

        public List<string> GetSingleTransfer(string transferId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transaction/transfers/specific/" + transferId);
            IRestResponse<List<string>> response = client.Get<List<string>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {

                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);

            }
            else
            {
                return response.Data;
            }
        }

        public int GetTransferId()
        {
            RestRequest request = new RestRequest (API_BASE_URL + "transaction/transferId");
            IRestResponse<int> response = client.Get<int>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {

                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);

            }
            else
            {
                return response.Data;
            }
        }
        //write server call to collect transfer id (Steve)
    }
}
