
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LibraryManagement.Models;

namespace LibraryManagement.Filters
{
    public class BookValidationFilter : IActionFilter
    {
        /*Book title must not be empty.
        Author must not be empty.
    Price must be greater than zero.
    If validation fails, return 400 Bad Request.
    If validation succeeds, allow the controller action to execute.*/ /*Book title must not be empty.
    Author must not be empty.
     Price must be greater than zero.
    If validation fails, return 400 Bad Request.
    If validation succeeds, allow the controller action to execute.*/
        public void OnActionExecuting(
            ActionExecutingContext context)
        {
            // Get the Book object passed to the action
            var book = context.ActionArguments.Values
                .OfType<Book>()
                .FirstOrDefault();

            // If no Book argument is found
            if (book == null)
            {
                context.Result = new BadRequestObjectResult(
                    "Book data is required");

                return;
            }

            // Validate Title
            if (string.IsNullOrWhiteSpace(book.Title))
            {
                context.Result = new BadRequestObjectResult(
                    "Book title cannot be empty");

                return;
            }

            // Validate Author
            if (string.IsNullOrWhiteSpace(book.Author))
            {
                context.Result = new BadRequestObjectResult(
                    "Book author cannot be empty");

                return;
            }

            // Validate Price
            if (book.Price <= 0)
            {
                context.Result = new BadRequestObjectResult(
                    "Book price must be greater than zero");

                return;
            }

            Console.WriteLine("Book validation successful");
        }

        public void OnActionExecuted(
            ActionExecutedContext context)
        {
            Console.WriteLine("AddBook action execution completed");
        }
    }
}

