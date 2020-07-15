using System;
using System.Collections.Generic;
using TenmoClient.Data;


namespace TenmoClient
{
    public class ConsoleService
    {
        private static readonly AuthService authService = new AuthService();
        string username;

        public void Run()
        {

            while (true)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.WriteLine("3: Exit");
                Console.Write("Please choose an option: ");

                int loginRegister = -1;

                try
                {
                    if (!int.TryParse(Console.ReadLine(), out loginRegister))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }

                    else if (loginRegister == 1)
                    {
                        LoginUser loginUser = PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                            MenuSelection();
                        }
                    }

                    else if (loginRegister == 2)
                    {
                        LoginUser registerUser = PromptForLogin();
                        bool isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                        }
                    }

                    else if (loginRegister == 3)
                    {
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                    }

                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error - " + ex.Message);
                }
            }
        }

        private void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers"); //view details through here
                Console.WriteLine("3: View your pending requests"); //ability to approve/reject through here
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: View list of users");
                Console.WriteLine("7: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                try
                {
                    if (!int.TryParse(Console.ReadLine(), out menuSelection))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }
                    else if (menuSelection == 1)
                    {
                        int userId = UserService.GetUserId();
                        int accountBalance = authService.GetBalance(userId);
                        Console.WriteLine("Account balance = " + accountBalance);
                    }
                    else if (menuSelection == 2)
                    {
                        Console.WriteLine("Enter transfer Id or enter 0 to see all of your past transactions");
                        string userInput = Console.ReadLine();
                        if (userInput == "0")
                        {
                            int userId = UserService.GetUserId();
                            List<string> transferDetails = authService.GetAllTransfers(userId);
                            foreach (string item in transferDetails)
                            {
                                Console.WriteLine("Transfer Details: " + item);
                            }
                        }
                        else
                        {
                            
                            List<string> transferDetails = authService.GetSingleTransfer(userInput);
                            foreach (string item in transferDetails)
                            {
                                Console.WriteLine("Transfer Details: " + item);
                            }
                        }

                    }
                    else if (menuSelection == 3)
                    {

                    }
                    else if (menuSelection == 4)
                    {
                        decimal transferAmount = 0;
                        Console.WriteLine("Please enter the amount of TE Bucks to send:");
                        string stringTransferAmount = Console.ReadLine();

                        try
                        {
                            transferAmount = Convert.ToDecimal(stringTransferAmount);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine(Environment.NewLine + "ERROR: Please enter an integer amount to be transferred");
                        }

                        Console.WriteLine("Please enter the User ID of the recipient");

                        string stringtransferRecipientId = Console.ReadLine();
                        int transferRecipientId = 0;
                        try
                        {
                            transferRecipientId = Convert.ToInt32(stringtransferRecipientId);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine(Environment.NewLine + "ERROR: Please enter an ID using only integers");
                        }

                        int userId = UserService.GetUserId();

                        bool addFundsSuccess = authService.AddFunds(transferAmount, transferRecipientId);
                        bool transferSuccess = authService.TransferFunds(transferAmount, userId, transferRecipientId);

                        if (transferSuccess == true && addFundsSuccess == true)
                        {
                            int transferId = authService.GetTransferId();
                            Console.WriteLine("transfer successful");
                            Console.WriteLine("Please keep for your records. Transfer Id = " + transferId);
                        }
                        else
                        {
                            Console.WriteLine("transfer unable to be completed successfully");
                        }
                    }
                    else if (menuSelection == 5)
                    {

                    }
                    else if (menuSelection == 6)
                    {
                        string users = authService.GetUsers();
                        Console.WriteLine(users);

                    }
                    else if (menuSelection == 7)
                    {
                        Console.WriteLine("");
                        UserService.SetLogin(new API_User()); //wipe out previous login info
                        return; //return to register/login menu
                    }
                    else if (menuSelection == 0)
                    {
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                    }

                    else
                    {
                        Console.WriteLine("Please try again");
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error - " + ex.Message);
                    Console.WriteLine();
                }
            }
        }

        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }
    }
}
