using System;
using System.Collections.Generic;
using System.Text;

namespace PencilBox
{
    using Opexplicit = Definition.Opexplicit;
    using OpNode = Definition.OpNode;

    public class DSL
    {
        public delegate OpNode Func(params object[] args);
        public delegate Func FuncWithScope(params object[] args);

        static Func genFunc(Opexplicit index)
        {
            return (body_args) => new OpNode
            {
                opcode = (byte)index,
                arguments = null,
                body = new List<object>(body_args)
            };
        }

        static FuncWithScope genFuncWithScope(Opexplicit index)
        {
            return (args) =>
            {
                return (body_args) =>
                {
                    return new OpNode
                    {
                        opcode = (byte)index,
                        arguments = new List<object>(args),
                        body = new List<object>(body_args)
                    };
                };
            };
        }

        public FuncWithScope func = genFuncWithScope(Opexplicit.func);
        public FuncWithScope scope = genFuncWithScope(Opexplicit.scope);

        public Func get = genFunc(Opexplicit.get);
        public Func apply = genFunc(Opexplicit.apply);
        public Func print = genFunc(Opexplicit.print);

        public Func add = genFunc(Opexplicit.add);
        public Func sub = genFunc(Opexplicit.sub);
        public Func div = genFunc(Opexplicit.div);
        public Func mul = genFunc(Opexplicit.mul);
        public Func mod = genFunc(Opexplicit.mod);

        public Func envGet = genFunc(Opexplicit.envGet);
        public Func envSet = genFunc(Opexplicit.envSet);

        public Func ifElse = genFunc(Opexplicit.ifElse);

        public Func eq = genFunc(Opexplicit.eq);
        public Func gt = genFunc(Opexplicit.gt);
        public Func ge = genFunc(Opexplicit.ge);
        public Func lt = genFunc(Opexplicit.lt);
        public Func le = genFunc(Opexplicit.le);

        public Func list = genFunc(Opexplicit.list);
        public Func index = genFunc(Opexplicit.index);
        public Func pop = genFunc(Opexplicit.pop);
        public Func push = genFunc(Opexplicit.push);
        public Func shift = genFunc(Opexplicit.shift);
        public Func unshift = genFunc(Opexplicit.unshift);

        public Func canvas = genFunc(Opexplicit.canvas);
        public Func fillStyle = genFunc(Opexplicit.fillStyle);
        public Func font = genFunc(Opexplicit.font);
        public Func globalAlpha = genFunc(Opexplicit.globalAlpha);
        public Func globalCompositeOperation = genFunc(Opexplicit.globalCompositeOperation);
        public Func lineCap = genFunc(Opexplicit.lineCap);
        public Func lineDashOffset = genFunc(Opexplicit.lineDashOffset);
        public Func lineJoin = genFunc(Opexplicit.lineJoin);
        public Func lineWidth = genFunc(Opexplicit.lineWidth);
        public Func miterLimit = genFunc(Opexplicit.miterLimit);
        public Func shadowBlur = genFunc(Opexplicit.shadowBlur);
        public Func shadowColor = genFunc(Opexplicit.shadowColor);
        public Func shadowOffsetX = genFunc(Opexplicit.shadowOffsetX);
        public Func shadowOffsetY = genFunc(Opexplicit.shadowOffsetY);
        public Func strokeStyle = genFunc(Opexplicit.strokeStyle);
        public Func textAlign = genFunc(Opexplicit.textAlign);
        public Func textBaseline = genFunc(Opexplicit.textBaseline);

        public Func arc = genFunc(Opexplicit.arc);
        public Func arcTo = genFunc(Opexplicit.arcTo);
        public Func beginPath = genFunc(Opexplicit.beginPath);
        public Func bezierCurveTo = genFunc(Opexplicit.bezierCurveTo);
        public Func clearRect = genFunc(Opexplicit.clearRect);
        public Func clip = genFunc(Opexplicit.clip);
        public Func closePath = genFunc(Opexplicit.closePath);
        public Func createImageData = genFunc(Opexplicit.createImageData);
        public Func createLinearGradient = genFunc(Opexplicit.createLinearGradient);
        public Func addColorStop = genFunc(Opexplicit.addColorStop);
        public Func createPattern = genFunc(Opexplicit.createPattern);
        public Func createRadialGradient = genFunc(Opexplicit.createRadialGradient);
        public Func drawImage = genFunc(Opexplicit.drawImage);
        public Func ellipse = genFunc(Opexplicit.ellipse);
        public Func fill = genFunc(Opexplicit.fill);
        public Func fillRect = genFunc(Opexplicit.fillRect);
        public Func fillText = genFunc(Opexplicit.fillText);
        public Func getImageData = genFunc(Opexplicit.getImageData);
        public Func getLineDash = genFunc(Opexplicit.getLineDash);
        public Func isPointInPath = genFunc(Opexplicit.isPointInPath);
        public Func isPointInStroke = genFunc(Opexplicit.isPointInStroke);
        public Func lineTo = genFunc(Opexplicit.lineTo);
        public Func measureText = genFunc(Opexplicit.measureText);
        public Func moveTo = genFunc(Opexplicit.moveTo);
        public Func putImageData = genFunc(Opexplicit.putImageData);
        public Func quadraticCurveTo = genFunc(Opexplicit.quadraticCurveTo);
        public Func rect = genFunc(Opexplicit.rect);
        public Func restore = genFunc(Opexplicit.restore);
        public Func rotate = genFunc(Opexplicit.rotate);
        public Func save = genFunc(Opexplicit.save);
        public Func scale = genFunc(Opexplicit.scale);
        public Func setLineDash = genFunc(Opexplicit.setLineDash);
        public Func setTransform = genFunc(Opexplicit.setTransform);
        public Func stroke = genFunc(Opexplicit.stroke);
        public Func strokeRect = genFunc(Opexplicit.strokeRect);
        public Func strokeText = genFunc(Opexplicit.strokeText);
        public Func transform = genFunc(Opexplicit.transform);
        public Func translate = genFunc(Opexplicit.translate);

    }
}
