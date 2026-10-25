var allMethods = QObjectType.All.SelectMany(x => x.Methods).OrderBy(x => x.Id).ToArray();
var idCounter = 0;
var keysByMethod = allMethods.ToDictionary(x => x, x => idCounter++);

Generator.GenerateCs(args[0]);
Generator.GenerateCxx(args[1]);