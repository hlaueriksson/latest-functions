﻿using LatestFunctions.Queries;
using Machine.Specifications;

namespace LatestFunctions.Specs.Queries
{
    [Subject(typeof(GitHubQueryHandler))]
    public class GitHubQueryHandlerSpecs
    {
        Establish context = () => Subject = new GitHubQueryHandler(new Configuration { GitHubQueryHandlerUsername = "hlaueriksson" });

        Because of = () => Result = Subject.HandleAsync(new GitHubQuery()).Result;

        It should_return_data_about_the_latest_created_repo = () =>
        {
            Result.Repo.ShouldNotBeNull();
        };

        It should_return_data_about_the_latest_commit = () =>
        {
            Result.Commit.ShouldNotBeNull();
        };

        static GitHubQueryHandler Subject;
        static GitHubData Result;
    }
}