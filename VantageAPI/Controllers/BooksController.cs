using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using VantageAPI.Library;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VantageAPI.Controllers
{
    [Route("api/book/")]
    [ApiController]
    public class BooksController : ControllerBase
    {


        // POST api/<BooksController>
        [HttpPost("create")]
      
        public int CreateBook(Book book)
        {

           

            List<string> acceptedCategories = new List<string> { "thriller", "history", "drama", "biography" };
            try
            {
                
                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-HBS43OB\SQLEXPRESS01;Initial Catalog=tempdb;Integrated Security=True"))
                {
                    con.Open();

                    // Checking if book ID already exists
                    string checkExistID = "SELECT COUNT(*) FROM TBbook WHERE bookid = @BOOKID";
                    SqlCommand checkExistCommand = new SqlCommand(checkExistID, con);
                    checkExistCommand.Parameters.AddWithValue("@BOOKID", book.BookId);

                    int checkBookIdExist = (int)checkExistCommand.ExecuteScalar();

                    if (checkBookIdExist > 0)
                    {
                        throw new Exception("Book ID already exists");
                    }

                    // Generating new book ID
                    string maxBookIdQuery = "SELECT MAX(BOOKID) FROM TBbook";
                    SqlCommand maxBookIdCommand = new SqlCommand(maxBookIdQuery, con);

                    object maxBookIdResult = maxBookIdCommand.ExecuteScalar();
                    int maxBookId = 0;

                    if (maxBookIdResult != DBNull.Value)
                    {
                        maxBookId = Convert.ToInt32(maxBookIdResult);
                    }
                    book.BookId = maxBookId + 1;

                    
                    con.Close();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            // To add data in the Table through Stored Precedure
            using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-HBS43OB\SQLEXPRESS01;Initial Catalog=tempdb;Integrated Security=True"))
            {


                con.Open();
                SqlCommand cmd = new SqlCommand("CreateBook", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BOOKID", book.BookId);
                cmd.Parameters.AddWithValue("@BOOKNAME", book.BookName);
                cmd.Parameters.AddWithValue("@AUTHOR", book.Author);
                cmd.Parameters.AddWithValue("@REGISTRATIONTIMESTAMP", book.RegistrationTimestamp = DateTime.Now);
                if (acceptedCategories.Contains(book.Category))
                {
                    cmd.Parameters.AddWithValue("@CATEGORY", book.Category);
                }
                else
                {
                    throw new Exception("Invalid category. Accepted categories are: thriller, history, drama, biography.");
                }
                cmd.Parameters.AddWithValue("@DESCRIPTION", book.Description);


                cmd.ExecuteNonQuery();

                con.Close();
            }

            return (book.BookId);
        }









        // PUT api/<BooksController>/5

        [HttpPut("bookId/update")]


        public ActionResult<Book> UpdateBook(int bookid, Book updateBook)
        {
            try
            {
                // Checking if any null or null values are applied as i/p
                if (bookid < 0)
                {
                    throw new Exception("Please enter a valid BookID");
                }

                if (updateBook == null)
                {
                    throw new Exception($"Book with ID {bookid} does not exist");
                }


                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-HBS43OB\SQLEXPRESS01;Initial Catalog=tempdb;Integrated Security=True"))
                {
                    con.Open();

                    // Create a command to retrieve the ID from the database
                    string query = "SELECT BOOKID FROM TBbook WHERE bookid = @BOOKID";
                    SqlCommand command = new SqlCommand(query, con);
                    command.Parameters.AddWithValue("@BOOKID", bookid);

                    // Execute the query and check if the user ID exists in the database
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Book ID exists in the database
                            Console.WriteLine("Book ID exists in the database.");

                            // To add updated data in the Table through Stored Precedure


                            SqlCommand cmd = new SqlCommand("UpdateBook", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@BOOKID", updateBook.BookId));
                            cmd.Parameters.Add(new SqlParameter("@BOOKNAME", updateBook.BookName));
                            cmd.Parameters.Add(new SqlParameter("@AUTHOR", updateBook.Author));
                            cmd.Parameters.Add(new SqlParameter("@REGISTRATIONTIMESTAMP", updateBook.RegistrationTimestamp));

                            cmd.Parameters.AddWithValue("@CATEGORY", updateBook.Category);
                            cmd.Parameters.AddWithValue("@DESCRIPTION", updateBook.Description);
                            reader.Close();
                                cmd.ExecuteNonQuery();
                        }

                        

                    }


                    
                    con.Close();

                }


                return Ok(new
                {
                    updateBook.BookId,
                    updateBook.BookName,
                    updateBook.Author,
                    updateBook.RegistrationTimestamp,
                    updateBook.Category,
                    updateBook.Description
                });
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}   


           

          




      



