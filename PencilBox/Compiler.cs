using System;
using System.Collections.Generic;
using System.Text;

namespace PencilBox
{
    using Opdata = Definition.Opdata;
    using Opimplicit = Definition.Opimplicit;
    using Opexplicit = Definition.Opexplicit;
    using OpNode = Definition.OpNode;

    public class Compiler
    {
        // The compiler will collect all the string contents
        // and encode them to bytes to store in this byte list
        List<byte> textstack_bytes;

        // This is the string-index mapping table for `textstack_bytes`
        Dictionary<string, uint> textstack_map;
        uint textstack_index;

        // All the encoded program will be stored in this byte list
        List<byte> bytecodes;

        public Compiler()
        {
            this.textstack_bytes = new List<byte>(new byte[] { (byte)Opimplicit.textstack, 0, 0, 0, 0 });
            this.textstack_map = new Dictionary<string, uint>();
            this.textstack_index = 0;
            this.bytecodes = new List<byte>();
        }

        static Dictionary<Type, byte> NumericMap = new Dictionary<Type, byte>
        {
            { typeof(sbyte), (byte)Opdata.int8 },
            { typeof(byte), (byte)Opdata.uint8 },
            { typeof(short), (byte)Opdata.int16 },
            { typeof(ushort), (byte)Opdata.uint16 },
            { typeof(int), (byte)Opdata.int32 },
            { typeof(uint), (byte)Opdata.uint32 },
            { typeof(long), (byte)Opdata.float64 },
            // { typeof(ulong), (byte)Opdata.float64 }, UInt64 is not supported by ECMAScript 2015
            { typeof(float), (byte)Opdata.float32 },
            { typeof(double), (byte)Opdata.float64 }
        };

        delegate byte[] NumericCovert();
        void walkAST(object ast_node, List<string> varstack, byte varstack_callindex)
        {
            Type t = ast_node.GetType();

            if (NumericMap.ContainsKey(t))
            {

                this.bytecodes.Add(NumericMap[t]);

                Dictionary<Type, NumericCovert> ConvertMap = new Dictionary<Type, NumericCovert>
                {
                    { typeof(sbyte), () => BitConverter.GetBytes((sbyte)ast_node) },
                    { typeof(byte), () => BitConverter.GetBytes((byte)ast_node) },
                    { typeof(short), () => BitConverter.GetBytes((short)ast_node) },
                    { typeof(ushort), () => BitConverter.GetBytes((ushort)ast_node) },
                    { typeof(int), () => BitConverter.GetBytes((int)ast_node) },
                    { typeof(uint), () => BitConverter.GetBytes((uint)ast_node) },
                    { typeof(long), () => BitConverter.GetBytes((long)ast_node) },
                    { typeof(float), () => BitConverter.GetBytes((float)ast_node) },
                    { typeof(double), () => BitConverter.GetBytes((double)ast_node) }
                };

                foreach (byte item in ConvertMap[t]())
                    this.bytecodes.Add(item);

            }
            else if (t == typeof(string))
            {

                string node = (string)ast_node;

                this.bytecodes.Add((byte)Opdata.iot);

                if (this.textstack_map.ContainsKey(node))
                    this.walkAST(this.textstack_map[node], null, 0);
                else
                {
                    foreach (byte item in Encoding.BigEndianUnicode.GetBytes(node))
                        this.textstack_bytes.Add(item);

                    this.textstack_bytes.Add(Definition.EOT);
                    this.textstack_bytes.Add(0);

                    this.textstack_map[node] = this.textstack_index;

                    this.walkAST(this.textstack_index, null, 0);

                    this.textstack_index += 1;
                }

            }
            else if (t == typeof(OpNode))
            {

                OpNode node = (OpNode)ast_node;

                if (node.opcode == (byte)Opexplicit.scope)
                {
                    // node.arguments[0] is the variable name
                    // node.arguments[1] is the content
                    if (node.arguments.Count >= 2)
                    {
                        varstack.Add((string)node.arguments[0]);

                        this.walkAST(node.arguments[1], varstack, varstack_callindex);

                        this.bytecodes.Add(node.opcode);
                    }

                    foreach (object item in node.body)
                        this.walkAST(item, varstack, varstack_callindex);

                    if (node.arguments.Count >= 2)
                    {
                        varstack.RemoveAt(varstack.Count - 1);
                        this.bytecodes.Add((byte)Opimplicit.sweep);
                    }

                }
                else if (node.opcode == (byte)Opexplicit.get)
                {

                    string name = (string)node.body[0];
                    bool not_found = true;

                    for (int l = varstack.Count; l-- != 0;)
                    {
                        if (name == varstack[l])
                        {
                            if (l < varstack_callindex)
                            {
                                this.bytecodes.Add(node.opcode);
                                this.bytecodes.Add((byte)l);
                            }
                            else
                            {
                                this.bytecodes.Add((byte)Opimplicit.localfget);
                                this.bytecodes.Add((byte)(l - varstack_callindex));
                            }

                            not_found = false;
                            break;
                        }
                    }

                    if (not_found)
                        throw new Exception("no variable " + name + " in scope");

                }
                else if (node.opcode == (byte)Opexplicit.func)
                {

                    this.bytecodes.Add(node.opcode);

                    // ======= set the length of function body ========
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);

                    // ======= push variables to var stack ========
                    int bytecodes_len = this.bytecodes.Count;

                    this.bytecodes.Add((byte)node.arguments.Count);

                    varstack_callindex = (byte)varstack.Count;

                    foreach (string item in node.arguments)
                        varstack.Add(item);

                    // ========= walk through function body ==========
                    foreach (object item in node.body)
                        this.walkAST(item, varstack, varstack_callindex);

                    // ========= jump back to origin pc =============
                    this.bytecodes.Add((byte)Opimplicit.sweepn);
                    this.bytecodes.Add((byte)Opimplicit.jump);

                    // ========= clean up ===========
                    int args_len = node.arguments.Count;
                    while (args_len != 0)
                    {
                        args_len -= 1;
                        varstack.RemoveAt(varstack.Count - 1);
                    }

                    // ========= set function body length ============
                    byte[] offset_bytes = BitConverter.GetBytes((uint)(this.bytecodes.Count - bytecodes_len));
                    this.textstack_bytes[bytecodes_len - 4] = offset_bytes[0];
                    this.textstack_bytes[bytecodes_len - 3] = offset_bytes[1];
                    this.textstack_bytes[bytecodes_len - 2] = offset_bytes[2];
                    this.textstack_bytes[bytecodes_len - 1] = offset_bytes[3];

                }
                else if (node.opcode == (byte)Opexplicit.ifElse)
                {

                    // push condition on stack
                    this.walkAST(node.body[0], varstack, varstack_callindex);

                    this.bytecodes.Add(node.opcode);

                    // else branch addresss
                    int else_val_address = this.bytecodes.Count;
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);

                    // body of branch true
                    this.walkAST(node.body[1], varstack, varstack_callindex);

                    // exit address
                    this.bytecodes.Add((byte)Opdata.uint32);

                    int exit_val_address = this.bytecodes.Count;
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);
                    this.bytecodes.Add(0);

                    this.bytecodes.Add((byte)Opimplicit.jumpoffset);

                    byte[] offset_bytes = BitConverter.GetBytes((uint)(this.bytecodes.Count - else_val_address));
                    this.bytecodes[else_val_address + 0] = offset_bytes[0];
                    this.bytecodes[else_val_address + 1] = offset_bytes[1];
                    this.bytecodes[else_val_address + 2] = offset_bytes[2];
                    this.bytecodes[else_val_address + 3] = offset_bytes[3];

                    // body of branch false
                    this.walkAST(node.body[2], varstack, varstack_callindex);

                    byte[] exit_address_bytes = BitConverter.GetBytes((uint)(this.bytecodes.Count - (exit_val_address + 5)));
                    this.bytecodes[exit_val_address + 0] = exit_address_bytes[0];
                    this.bytecodes[exit_val_address + 1] = exit_address_bytes[1];
                    this.bytecodes[exit_val_address + 2] = exit_address_bytes[2];
                    this.bytecodes[exit_val_address + 3] = exit_address_bytes[3];

                }
                else
                {

                    foreach (object item in node.body)
                        this.walkAST(item, varstack, varstack_callindex);

                    this.bytecodes.Add(node.opcode);

                    if (Enum.IsDefined(typeof(Opexplicit), node.opcode) && !Definition.OpWithTwoArgs.Contains((Opexplicit)node.opcode))
                        this.bytecodes.Add((byte)node.body.Count);

                }
            }
        }

        public void compile(params object[] ast_nodes)
        {
            foreach (object ast_node in ast_nodes)
            {
                this.walkAST(ast_node, new List<string>(), byte.MaxValue);

                byte[] text_length_bytes = BitConverter.GetBytes((uint)this.textstack_bytes.Count - 1);
                this.textstack_bytes[1] = text_length_bytes[0];
                this.textstack_bytes[2] = text_length_bytes[1];
                this.textstack_bytes[3] = text_length_bytes[2];
                this.textstack_bytes[4] = text_length_bytes[3];
            }
        }

        public byte[] output()
        {
            List<byte> result = new List<byte>();
            result.AddRange(this.textstack_bytes);
            result.AddRange(this.bytecodes);
            return result.ToArray();
        }
    }
}
