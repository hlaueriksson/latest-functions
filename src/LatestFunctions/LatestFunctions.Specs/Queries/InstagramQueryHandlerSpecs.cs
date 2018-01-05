using LatestFunctions.Queries;
using Machine.Specifications;

namespace LatestFunctions.Specs.Queries
{
    [Subject(typeof(InstagramQueryHandler))]
    public class InstagramQueryHandlerSpecs
    {
        Establish context = () => Subject = new InstagramQueryHandler();

        Because of = () => Result = Subject.HandleAsync(new InstagramQuery()).Result;

        It should_return_HTML_for_the_latest_photo = () =>
        {
            Result.Html.ShouldNotBeEmpty();
        };

        static InstagramQueryHandler Subject;
        static InstagramData Result;
    }
}