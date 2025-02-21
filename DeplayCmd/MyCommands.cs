using System.CommandLine;
using BaseLib;
using DeplaymentLib;
using DeplaymentLib.Project;

namespace DeplayCmd;

public class MyCommands
{
    private const string optionDoesNotExistDefaultValue = "###option-doaes-not-exist###";

    /// <summary>
    /// 
    /// </summary>
    private RootCommand RootCommand { get; set; }

    /// <summary>
    /// 
    /// </summary>
    private string[] ApplicationArguments { get; set; }

    /// <summary>
    /// 
    /// </summary>
    private enum Errors
    {
        MissingArgument = 1,
        ProjectNotFound,
        Unexpected = 99
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    public MyCommands(string[] args)
    {
        Env.CheckInstance();

        ApplicationArguments = args;

        // var delayArgument = new Argument<int>
        // (name: "delay",
        //     description: "An argument that is parsed as an int.",
        //     getDefaultValue: () => 42);
        // var messageArgument = new Argument<string>
        //     ("message", "An argument that is parsed as a string.");

        RootCommand = new RootCommand("The root command");
        RootCommand.SetHandler(() => { Console.Write("Root started."); });


        // command: list
        {
            var subCommandList = new Command("list", "List projects");

            // option --project-path (-p)
            var optionProjectPath = new Option<string?>("--project-path");
            optionProjectPath.AddAlias("-p");
            subCommandList.AddOption(optionProjectPath);

            RootCommand.Add(subCommandList);
            subCommandList.SetHandler((optionProjectPathValue) =>
            {
                // explicit project path?
                if (!string.IsNullOrEmpty(optionProjectPathValue))
                {
                    Env.PathProjects = optionProjectPathValue;
                }

                var projects = Env.ListProjects().OrderBy(x => x.Name).ToList();
                Console.WriteLine($"Found {projects.Count} valid projects:");
                Console.WriteLine("");
                var index = 0;
                foreach (var project in projects)
                {
                    var strDeployments = string.Join(",", project.EnabledDeployments.Select(x => x.Code));
                    // Console.WriteLine($"{++index}) {project.Name} (enabled deployments: {project.DeploymentContainers?.Sum(x => x.EnabledDeployments.Count) ?? 0}, Variants: {project.LaunchVariants?.Count ?? 0})");
                    Console.WriteLine(
                        $"{++index}) \"{project.Name}\" (enabled deployments: {project.EnabledDeployments.Count} [{strDeployments}], Variants: {project.LaunchVariants?.Count ?? 0})");
                    Console.WriteLine($"   {project.Description}");
                    Console.WriteLine("");
                }
            }, optionProjectPath);
        }
        // command: run
        {
            var subCommandRun = new Command("run", "Run a project");
            var argProject = new Argument<string>
                ("project", "Project name (with optional file extension)");
            var argVariant = new Argument<string>
                ("variant", () => "", "Project Variant");
            subCommandRun.Add(argProject);
            subCommandRun.Add(argVariant);

            // option --simulate (-s)
            var optionSimulate = new Option<bool?>("--simulate", () => null);
            optionSimulate.AddAlias("-s");
            subCommandRun.AddOption(optionSimulate);

            // option --debug-level (-d)
            var optionDebugLevel = new Option<string?>("--debug-level");
            optionDebugLevel.AddAlias("-d");
            subCommandRun.AddOption(optionDebugLevel);

            // option --project-path (-p)
            var optionProjectPath = new Option<string?>("--project-path");
            optionProjectPath.AddAlias("-p");
            subCommandRun.AddOption(optionProjectPath);

            RootCommand.Add(subCommandRun);
            subCommandRun.SetHandler((projectName, variantName, optionSimulateValue, optionDebugLevelValue,
                    optionProjectPathValue) =>
                {
                    // var x = 0.123456789;
                    // Console.WriteLine($"{x,6:F2}");
                    // return;

                    Console.WriteLine($"Project: \"{projectName}\"");
                    Console.WriteLine($"Variant: \"{variantName}\"");
                    if (optionSimulateValue == true) Console.WriteLine("Simulate enabled.");
                    Console.WriteLine($"DebugLevel: \"{optionDebugLevelValue}\"");
                    // Console.WriteLine($"Option: \"{((optionSimulateValue is not null) ? "nothing" : "no")}\"");

                    // explicit project path?
                    if (!string.IsNullOrEmpty(optionProjectPathValue))
                    {
                        Env.PathProjects = optionProjectPathValue;
                    }

                    // load the project
                    var project = Env.LoadProject(projectName);
                    if (project is null)
                    {
                        Console.WriteLine($"Failed to load project: '{projectName}'.");
                        Environment.Exit((int)Errors.ProjectNotFound);
                    }

                    // Assign debug level option after project was loaded to avoid project override
                    if (Enum.TryParse<LogDebugLevel>(optionDebugLevelValue, out var parsedDebugLevel))
                    {
                        Env.Log.DebugLevel = parsedDebugLevel;
                    }

                    if (variantName != "")
                    {
                        project.LaunchVariantSelected = variantName;
                    } // Otherwise, use the configured one (if exists)


                    // assign simulate option to project
                    // @todo: find a way for option exists
                    if (optionSimulateValue is not null)
                        project.Simulate = optionSimulateValue ?? false;


                    // all messages to console ...
                    project.SetOnMessageDelegate((type, msg) => { Console.WriteLine($"{type}: {msg}"); });

                    Console.WriteLine($"Total Deployment Containers found: {project.DeploymentContainers.Count}");
                    Console.WriteLine(
                        $"Enabled Project Deployments: {project.EnabledDeployments.Count} - {string.Join(",", project.EnabledDeployments.Select(x => x.Code))}");

                    // Run the project
                    if (!project.Run())
                    {
                        Environment.Exit((int)Errors.Unexpected);
                    }

                    // Print deployments messages
                    // Console.WriteLine();
                    // PrintDeploymentsMessages(project);
                },
                argProject, argVariant, optionSimulate, optionDebugLevel, optionProjectPath);
        }
        // command: show
        {
            var subCommandShow = new Command("show", "Show project information");
            var argProject = new Argument<string>
                ("project", "Project name (with optional file extension)");
            subCommandShow.Add(argProject);

            // option --project-path (-p)
            var optionProjectPath = new Option<string?>("--project-path");
            optionProjectPath.AddAlias("-p");
            subCommandShow.AddOption(optionProjectPath);

            RootCommand.Add(subCommandShow);
            subCommandShow.SetHandler((projectName, optionProjectPathValue) =>
                {
                    Console.WriteLine($"Project: {projectName}");

                    // explicit project path?
                    if (!string.IsNullOrEmpty(optionProjectPathValue))
                    {
                        Env.PathProjects = optionProjectPathValue;
                    }

                    var project = Env.LoadProject(projectName);
                    if (project is null)
                    {
                        Console.WriteLine($"Failed to load project: '{projectName}'.");
                        Environment.Exit((int)Errors.ProjectNotFound);
                    }

                    Console.WriteLine(
                        $"Enabled deployments: {project.EnabledDeployments.Count}/{project.DeploymentContainers?.Sum(x => x.Deployments.Count) ?? 0}");
                    Console.WriteLine($"Variants total: {project.LaunchVariants?.Count ?? 0}");
                    if (project.LaunchVariants?.Count > 0)
                    {
                        var index = 0;
                        foreach (var variant in project.LaunchVariants.OrderBy(x => x.Key))
                        {
                            Console.WriteLine($"{++index}) \"{variant.Key}\"");
                        }
                    }
                },
                argProject, optionProjectPath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Run()
    {
        RootCommand.Invoke(ApplicationArguments);
    }

    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="project"></param>
    // private void PrintDeploymentsMessages(Project project)
    // {
    //     foreach (var deployment in project.EnabledDeployments)
    //     {
    //         if (deployment.Messages.Count > 0)
    //         {
    //             Console.WriteLine($"Messages by {deployment.Code} - \"{deployment.Name}\":");
    //             foreach (var messageItem in deployment.Messages)
    //             {
    //                 Console.WriteLine($"{messageItem.Type}: {messageItem.Message}");
    //             }
    //
    //             Console.WriteLine();
    //         }
    //     }
    //
    // }
}