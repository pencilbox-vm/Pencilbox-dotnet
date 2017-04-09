# pencilbox-dotnet
**PencilBox** compiler in .NET Core

## Environment
.NET Core >= 1.1

## How to use

### Example: basic plotting
```c#

PencilBox.DSL op = new PencilBox.DSL();
PencilBox.Compiler compiler = new PencilBox.Compiler();

compiler = Compiler()
compiler.compile(
  op.beginPath(),
  op.rect(0, 0, 50, 50),
  op.arc(25, 25, 25, 0, 1.5 * 3.14159265359),
  op.closePath(),
  op.strokeStyle("red"),
  op.stroke()
)
bytecodes = compiler.output() // We get list of bytecodes here
```

### Example: fibonacci
```c#

PencilBox.DSL op = new PencilBox.DSL();
PencilBox.Compiler compiler = new PencilBox.Compiler();

var code = op.scope("fib",
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

compiler.compile(code);
var bytecodes = compiler.output() // We get list of bytecodes here
```

### Using bytecodes
Once the bytecodes is generated, it should be passed to **PencilBox** runtime in browser to run the program.
Please checkout the [**PencilBox** runtime `How to use`](https://github.com/pencilbox-vm/runtime#how-to-use) 
