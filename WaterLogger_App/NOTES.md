# NOTES FROM WORKING WITH RAZOR PAGES
1. Partials: files that can be reused in multiple pages.
2. ViewImports: used to centralize anything that will be reused repeatedly in our application to avoid repetition.
3. ViewStart: used to set a common layout for all views in the application.
4. "@": used to plug C# code into HTML.
5. "@page": used to indicate that the file is a Razor Page. It is the main difference between Razor Pages and MVC being that Razor Pages are page-based, while MVC is controller-based.
6. In Razor Pages, the name of the model will be the same as the name of the Razor Page, but with a "Model" suffix. For example, if the Razor Page is named "Index.cshtml", the model will be "IndexModel.cs".
7. asp-area: used to specify the area of the application that the page belongs to. It is used in conjunction with the "asp-page" attribute to link to other pages within the same area.
8. ADO.NET: a set of classes that expose data access services for .NET Framework programmers. It is used to connect to databases and execute SQL commands.
	- a data access solution provided by .NET and it forces you to write SQL queries.

9. When using Db Browser, after executing your SQL query to create a table, you have to
   - click on the "Write Changes" button to save the changes to the database.
   - click on the "Refresh" button to see the changes in the database.

10. When submitting a form, either use "<input type=submit>" or "<button type=submit>"
11. When navigating between pages, use "<a asp-page="/" >"