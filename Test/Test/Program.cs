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
                op.func('n')(
                    op.ifElse(
                    op.lt(op.get("n"), 2),
                    op.get("n"),
                    op.add(
                        op.apply(op.get("fib"), op.sub(op.get("n"), 1)),
                        op.apply(op.get("fib"), op.sub(op.get("n"), 2))))
                ))(
                op.print("dotnet fib pencilbox", op.apply(op.get("fib"), 30))
                );

            var ast2 = op.scope('f',
                  op.func('a', 'b')(
                    op.scope("f-nest",
                      op.func('x', 'y')(
                        op.add(op.get('x'), op.get('y'))
                      )
                    )(
                      op.apply(op.get("f-nest"), op.get('a'), op.get('b'))
                    )
                  )
                )(
                  op.print("dotnet eq: ", op.eq(9, op.apply(op.get('f'), 3, 6)))
                );

            compiler.compile(ast, ast2);

            Console.WriteLine(string.Join(",", compiler.output()));
            Console.Read();
        }
    }
}