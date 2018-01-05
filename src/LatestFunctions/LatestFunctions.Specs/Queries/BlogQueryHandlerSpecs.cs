using LatestFunctions.Queries;
using Machine.Specifications;

namespace LatestFunctions.Specs.Queries
{
    [Subject(typeof(BlogQueryHandler))]
    public class BlogQueryHandlerSpecs
    {
        Establish context = () => Subject = new BlogQueryHandler();

        Because of = () => Result = Subject.HandleAsync(new BlogQuery()).Result;

        It should_return_data_about_the_latest_blog_post = () =>
        {
            Result.Title.ShouldNotBeEmpty();
        };

        static BlogQueryHandler Subject;
        static BlogData Result;
    }
}