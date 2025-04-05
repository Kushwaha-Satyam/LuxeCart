using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Web.Helpers;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace LuxeCart.DatabaseWork.RegistrationDB
{
    public class UserRegistrationDB
    {
        private readonly LuxeCartEntities dbContext;
        public UserRegistrationDB()
        {
            dbContext = new LuxeCartEntities();
        }

        public bool UserRegisterationMethod(UserRegistration data)
        {
            // Validate input data
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "User registration data cannot be null.");
            }

            // Hash the password
            string hashedPassword = HashPassword(data.Password);

            try
            {
                // Create a new RegisterUser entity
                var insertUser = new RegisterUser()
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = data.Email,
                    PasswordHash = hashedPassword,
                    PhoneNumber = data.PhoneNumber,
                    DateOfBirth = data.DateOfBirth,
                    Gender = data.Gender,
                    ProfileImageUrl = data.ProfileImageUrl,
                    AcceptTerms = data.AcceptTerms,
                    CreatedAt = DateTime.Now,
                    Role = "User"
                };

                // Add the user to the database context
                dbContext.RegisterUsers.Add(insertUser);

                // Save changes to the database
                int result = dbContext.SaveChanges();

                // Return true if the user was successfully saved
                return result > 0;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Log validation errors
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}");
                        Console.WriteLine($"Property: {validationError.PropertyName}");
                        Console.WriteLine($"Error: {validationError.ErrorMessage}");
                    }
                }

                // Re-throw the exception or handle it as needed
                throw new Exception("User registration failed due to validation errors.", ex);
            }
            catch (Exception ex)
            {
                // Log other exceptions
                Console.WriteLine($"An error occurred during user registration: {ex.Message}");
                throw new Exception("User registration failed due to an unexpected error.", ex);
            }
        }

        public bool UserUpdateMethod(RegisterUser data)
        {
            try
            {
                var existingProduct = dbContext.RegisterUsers.SingleOrDefault(u => u.UserID == data.UserID);

                if (existingProduct == null)
                {
                    return false;
                }

                existingProduct.FirstName = data.FirstName;
                existingProduct.LastName = data.LastName;
                existingProduct.Email = data.Email;
                existingProduct.PhoneNumber = data.PhoneNumber;
                existingProduct.DateOfBirth = data.DateOfBirth;
                existingProduct.Gender = data.Gender;
                existingProduct.ProfileImageUrl = data.ProfileImageUrl;

                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Deleting User
        public bool DeleteUser(int UserID)
        {
            try
            {
                var user = dbContext.RegisterUsers.Find(UserID);
                if(user != null)
                {
                    dbContext.RegisterUsers.Remove(user);
                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return false;
        }

        //saving hashed password into database
        public string HashPassword(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }
        

        //verify password during login
        public bool VerifyPassword(string PasswordHash, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(PasswordHash, hashedPassword);
        }


        // List of allowed file extensions

        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        //inserting Image
        public string SaveProfileImage(HttpPostedFileBase file, string savePath)
        {
            if (file == null || file.ContentLength == 0)
            {
                throw new ArgumentException("Invalid image file.");
            }

            // Validate the file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) < 0)
            {
                throw new InvalidOperationException("Unsupported file format. Please upload a JPG, JPEG, PNG, or GIF file.");
            }

            // Generate a unique name for the image to prevent overwriting
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var folderPath = Path.GetDirectoryName(savePath);
            var finalPath = Path.Combine(folderPath, uniqueFileName);

            // Save the original file
            file.SaveAs(finalPath);

            return uniqueFileName;
        }

        public admin CheckAdminEmail(string email)
        {
            return dbContext.admins.FirstOrDefault(x => x.Email == email);
        }
        public RegisterUser CheckUserEmail(string email)
        {
            return dbContext.RegisterUsers.FirstOrDefault(u => u.Email == email);
        }

    }
}