# pencilbox-dotnet
pencilbox compiler in .NET Core

## Environment
.NET Core >= 1.1

## How to use
```c#

PencilBox.DSL op = new PencilBox.DSL();
PencilBox.Compiler compiler = new PencilBox.Compiler();

var ast = op.scope("fib",
    op.func("n")(
        op.ifElse(
        op.lt(op.get("n"), 2),
        op.get("n"),
        op.add(
            op.apply(op.get("fib"), op.sub(op.get("n"), 1)),
            op.apply(op.get("fib"), op.sub(op.get("n"), 2))))
    )
  )(
    op.print("dotnet fib pencilbox", op.apply(op.get("fib"), 30))
  );

compiler.compile(ast);
var bytecodes = compiler.output() # We get list of bytecodes here
```
Once the bytecodes is generated, it can be passed to pencilbox runtime in browser to run the program. 
