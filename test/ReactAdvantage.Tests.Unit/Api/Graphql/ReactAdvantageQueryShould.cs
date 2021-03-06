﻿using System;
using System.Linq;
using ReactAdvantage.Api.GraphQLSchema;
using ReactAdvantage.Domain.Models;
using Xunit;

namespace ReactAdvantage.Tests.Unit.Api.Graphql
{
    public class ReactAdvantageQueryShould : GraphqlTestBase
    {
        public ReactAdvantageQueryShould()
        {
            // Given
            using (var db = GetInMemoryDbContext())
            {
                db.Users.Add(new User { Id = 1, Name = "BobRay1", FirstName = "Bob", LastName = "Ray", Email = "BobRay@test.com", IsActive = true });
                db.Users.Add(new User { Id = 2, Name = "BobRay2", FirstName = "Bob", LastName = "Ray", Email = "BobRay@test.com", IsActive = false });
                db.Users.Add(new User { Id = 3, Name = "BobRay3", FirstName = "Bob", LastName = "Ray", Email = "BobRay@test2.com", IsActive = true });
                db.Users.Add(new User { Id = 4, Name = "BobRay4", FirstName = "Bob", LastName = "Ray", Email = "BobRay@test2.com", IsActive = false });
                db.Users.Add(new User { Id = 5, Name = "BobSmith1", FirstName = "Bob", LastName = "Smith", Email = "BobSmith@test.com", IsActive = true });
                db.Users.Add(new User { Id = 6, Name = "BobSmith2", FirstName = "Bob", LastName = "Smith", Email = "BobSmith@test.com", IsActive = false });
                db.Users.Add(new User { Id = 7, Name = "BobSmith3", FirstName = "Bob", LastName = "Smith", Email = "BobSmith@test2.com", IsActive = true });
                db.Users.Add(new User { Id = 8, Name = "BobSmith4", FirstName = "Bob", LastName = "Smith", Email = "BobSmith@test2.com", IsActive = false });
                db.Users.Add(new User { Id = 9, Name = "BarbaraRay1", FirstName = "Barbara", LastName = "Ray", Email = "BarbaraRay@test.com", IsActive = true });
                db.Users.Add(new User { Id = 10, Name = "BarbaraRay2", FirstName = "Barbara", LastName = "Ray", Email = "BarbaraRay@test.com", IsActive = false });
                db.Users.Add(new User { Id = 11, Name = "BarbaraRay3", FirstName = "Barbara", LastName = "Ray", Email = "BarbaraRay@test2.com", IsActive = true });
                db.Users.Add(new User { Id = 12, Name = "BarbaraRay4", FirstName = "Barbara", LastName = "Ray", Email = "BarbaraRay@test2.com", IsActive = false });
                db.Users.Add(new User { Id = 13, Name = "BarbaraSmith1", FirstName = "Barbara", LastName = "Smith", Email = "BarbaraSmith@test.com", IsActive = true });
                db.Users.Add(new User { Id = 14, Name = "BarbaraSmith2", FirstName = "Barbara", LastName = "Smith", Email = "BarbaraSmith@test.com", IsActive = false });
                db.Users.Add(new User { Id = 15, Name = "BarbaraSmith3", FirstName = "Barbara", LastName = "Smith", Email = "BarbaraSmith@test2.com", IsActive = true });
                db.Users.Add(new User { Id = 16, Name = "BarbaraSmith4", FirstName = "Barbara", LastName = "Smith", Email = "BarbaraSmith@test2.com", IsActive = false });
                db.SaveChanges();

                db.Projects.Add(new Project { Id = 1, Name = "Test Project 1" });
                db.Projects.Add(new Project { Id = 2, Name = "Test Project 2" });
                db.Projects.Add(new Project { Id = 3, Name = "Another Project 3" });
                db.SaveChanges();

                db.Tasks.Add(new Task { Id = 1, ProjectId = 1, Name = "Task 1", Description = "This is a test task", DueDate = new DateTime(2020, 1, 1) });
                db.Tasks.Add(new Task { Id = 2, ProjectId = 1, Name = "Task 2", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = true, CompletionDate = new DateTime(2010, 1, 1) });
                db.Tasks.Add(new Task { Id = 3, ProjectId = 1, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = true, CompletionDate = new DateTime(2010, 1, 1) });
                db.Tasks.Add(new Task { Id = 4, ProjectId = 2, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = true, CompletionDate = new DateTime(2010, 1, 1) });
                db.Tasks.Add(new Task { Id = 5, ProjectId = 1, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2001, 1, 1), Completed = true, CompletionDate = new DateTime(2010, 1, 1) });
                db.Tasks.Add(new Task { Id = 6, ProjectId = 1, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = false, CompletionDate = new DateTime(2010, 1, 1) });
                db.Tasks.Add(new Task { Id = 7, ProjectId = 1, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = true, CompletionDate = new DateTime(2011, 1, 1) });
                db.Tasks.Add(new Task { Id = 8, ProjectId = 1, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = true, CompletionDate = null });
                db.Tasks.Add(new Task { Id = 9, ProjectId = 1, Name = "Test Task Query", Description = "Another test task", DueDate = new DateTime(2000, 1, 1), Completed = true, CompletionDate = new DateTime(2010, 1, 1) });
                db.SaveChanges();
            }
        }

        [Fact]
        public async void ReturnUserWithFewFields()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { user(id: 1) { id name } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                userResult => AssertPairEqual(userResult, 
                    "user", user => AssertGraphqlResultDictionary(user,
                        field => AssertPairEqual(field, "id", 1),
                        field => AssertPairEqual(field, "name", "BobRay1")
                    )
                )
            );
        }

        [Fact]
        public async void ReturnUserWithMoreFields()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { user(id: 16) { id name firstName lastName email isActive } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                userResult => AssertPairEqual(userResult,
                    "user", user => AssertGraphqlResultDictionary(user,
                        field => AssertPairEqual(field, "id", 16),
                        field => AssertPairEqual(field, "name", "BarbaraSmith4"),
                        field => AssertPairEqual(field, "firstName", "Barbara"),
                        field => AssertPairEqual(field, "lastName", "Smith"),
                        field => AssertPairEqual(field, "email", "BarbaraSmith@test2.com"),
                        field => AssertPairEqual(field, "isActive", false)
                    )
                )
            );
        }

        [Fact]
        public async void ReturnQueriedUsers()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { users(firstname: \"Bo\", lastname: \"Sm\", email: \"@test2.com\", isactive: true) { id name firstName lastName email isActive } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                usersResult => AssertPairEqual(usersResult,
                    "users", users => AssertGraphqlResultArray(users, 
                        user => AssertGraphqlResultDictionary(user,
                            field => AssertPairEqual(field, "id", 7),
                            field => AssertPairEqual(field, "name", "BobSmith3"),
                            field => AssertPairEqual(field, "firstName", "Bob"),
                            field => AssertPairEqual(field, "lastName", "Smith"),
                            field => AssertPairEqual(field, "email", "BobSmith@test2.com"),
                            field => AssertPairEqual(field, "isActive", true)
                        )
                    )
                )
            );
        }

        [Fact]
        public async void ReturnQueriedUsersById()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { users(id: [1, 2, 3], isactive: true, ) { id name firstName lastName email isActive } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                usersResult => AssertPairEqual(usersResult,
                    "users", users => AssertGraphqlResultArray(users,
                        user => AssertGraphqlResultDictionary(user,
                            field => AssertPairEqual(field, "id", 1),
                            field => AssertPairEqual(field, "name", "BobRay1"),
                            field => AssertPairEqual(field, "firstName", "Bob"),
                            field => AssertPairEqual(field, "lastName", "Ray"),
                            field => AssertPairEqual(field, "email", "BobRay@test.com"),
                            field => AssertPairEqual(field, "isActive", true)
                        ),
                        user => AssertGraphqlResultDictionary(user,
                            field => AssertPairEqual(field, "id", 3),
                            field => AssertPairEqual(field, "name", "BobRay3"),
                            field => AssertPairEqual(field, "firstName", "Bob"),
                            field => AssertPairEqual(field, "lastName", "Ray"),
                            field => AssertPairEqual(field, "email", "BobRay@test2.com"),
                            field => AssertPairEqual(field, "isActive", true)
                        )
                    )
                )
            );
        }

        [Fact]
        public async void ReturnAllUsers()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { users { id name } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                usersResult => AssertPairEqual(usersResult,
                    "users", users =>
                    {
                        var usersArray = Assert.IsType<object[]>(users);
                        Assert.Equal(16, usersArray.Length);
                        AssertGraphqlResultDictionary(usersArray.First(),
                            field => AssertPairEqual(field, "id", 1),
                            field => AssertPairEqual(field, "name", "BobRay1")
                        );
                    }
                )
            );
        }

        [Fact]
        public async void ReturnProject()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { project(id: 2) { id name } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                projectResult => AssertPairEqual(projectResult,
                    "project", project => AssertGraphqlResultDictionary(project,
                        field => AssertPairEqual(field, "id", 2),
                        field => AssertPairEqual(field, "name", "Test Project 2")
                    )
                )
            );
        }

        [Fact]
        public async void ReturnAllProjects()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { projects { id name } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);
            
            AssertGraphqlResultDictionary(result.Data,
                projectsResult => AssertPairEqual(projectsResult,
                    "projects", projects =>
                    {
                        var projectsArray = Assert.IsType<object[]>(projects);
                        Assert.Equal(3, projectsArray.Length);
                        AssertGraphqlResultDictionary(projectsArray.First(),
                            field => AssertPairEqual(field, "id", 1),
                            field => AssertPairEqual(field, "name", "Test Project 1")
                        );
                    }
                )
            );
        }

        [Fact]
        public async void ReturnQueriedProjects()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { projects(id: [2, 3], name: \"Test\") { id name } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                projectsResult => AssertPairEqual(projectsResult,
                    "projects", projects => AssertGraphqlResultArray(projects,
                        project => AssertGraphqlResultDictionary(project,
                            field => AssertPairEqual(field, "id", 2),
                            field => AssertPairEqual(field, "name", "Test Project 2")
                        )
                    )
                )
            );
        }

        [Fact]
        public async void ReturnTask()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { task(id: 2) { id name description dueDate completed completionDate project { id name } } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                taskResult => AssertPairEqual(taskResult,
                    "task", task => AssertGraphqlResultDictionary(task,
                        field => AssertPairEqual(field, "id", 2),
                        field => AssertPairEqual(field, "name", "Task 2"),
                        field => AssertPairEqual(field, "description", "Another test task"),
                        field => AssertPairEqual(field, "dueDate", new DateTime(2000, 1, 1)),
                        field => AssertPairEqual(field, "completed", true),
                        field => AssertPairEqual(field, "completionDate", new DateTime(2010, 1, 1)),
                        field => AssertPairEqual(field, "project",
                            project => AssertGraphqlResultDictionary(project,
                                projectField => AssertPairEqual(projectField, "id", 1),
                                projectField => AssertPairEqual(projectField, "name", "Test Project 1")
                            )
                        )
                    )
                )
            );
        }

        [Fact]
        public async void ReturnAllTasks()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { tasks { id name } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                tasksResult => AssertPairEqual(tasksResult,
                    "tasks", tasks =>
                    {
                        var tasksArray = Assert.IsType<object[]>(tasks);
                        Assert.Equal(9, tasksArray.Length);
                        AssertGraphqlResultDictionary(tasksArray.First(),
                            field => AssertPairEqual(field, "id", 1),
                            field => AssertPairEqual(field, "name", "Task 1")
                        );
                    }
                )
            );
        }

        [Fact]
        public async void ReturnQueriedTasks()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { tasks(id: [1, 2, 3, 4, 5, 6, 7, 8], name: \"Test Task Query\", projectid: 1, iscompleted: true, duedate: \"2000-01-01\", completiondate: \"2010-01-01\") { id } }"
            });

            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                projectsResult => AssertPairEqual(projectsResult,
                    "tasks", tasks => AssertGraphqlResultArray(tasks,
                        task => AssertGraphqlResultDictionary(task,
                            field => AssertPairEqual(field, "id", 3)
                        )
                    )
                )
            );
        }

        [Fact]
        public async void ReturnTwoQueries()
        {
            // When
            var result = await BuildSchemaAndExecuteQueryAsync(new GraphQLQuery
            {
                Query = "query { user(id: 1) { name }, project(id: 1) { name } }"
            });
            
            // Then
            AssertValidGraphqlExecutionResult(result);

            AssertGraphqlResultDictionary(result.Data,
                userResult => AssertPairEqual(userResult,
                    "user", user => AssertGraphqlResultDictionary(user,
                        field => AssertPairEqual(field, "name", "BobRay1")
                    )
                ),
                projectResult => AssertPairEqual(projectResult,
                    "project", project => AssertGraphqlResultDictionary(project,
                        field => AssertPairEqual(field, "name", "Test Project 1")
                    )
                )
            );
        }
    }
}
