using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            PencilBox.DSL op = new PencilBox.DSL();
            PencilBox.Compiler compiler = new PencilBox.Compiler();

            var ast = op.scope("fib",
                op.func("n")(
                  op.ifElse(
                    op.lt(op.get("n"), 2),
                    op.get("n"),
                    op.add(
                      op.apply(op.get("fib"), op.sub(op.get("n"), 1)),
                      op.apply(op.get("fib"), op.sub(op.get("n"), 2))
                    )
                  )
                )
              )(
                op.print("dotnet fib pencilbox", op.apply(op.get("fib"), 30))
              );

            compiler.compile(ast);

            Console.WriteLine(string.Join(",", compiler.output()));
            Console.Read();
        }
    }
}