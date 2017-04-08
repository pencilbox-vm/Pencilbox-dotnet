using System;
using System.Collections.Generic;
using System.Text;

namespace PencilBox
{
    public class Definition
    {
        public struct OpNode
        {
            public byte opcode;
            public List<object> arguments;
            public List<object> body;
        }

        public static byte EOT = 3;

        public static HashSet<Opexplicit> OpWithTwoArgs = new HashSet<Opexplicit>
        {
            Opexplicit.eq,
            Opexplicit.gt,
            Opexplicit.ge,
            Opexplicit.lt,
            Opexplicit.le,

            Opexplicit.add,
            Opexplicit.sub,
            Opexplicit.mul,
            Opexplicit.div,
            Opexplicit.mod
        };

        public enum Opdata : byte
        {
            sot = 0, // start of classic text: [sot content EOT]
            iot, // index of textstack
            int8,
            uint8,
            int16,
            uint16,
            int32,
            uint32,
            float32,
            float64
        }

        public enum Opimplicit : byte
        {
            instrs = 15,
            textstack, // the mark of string stack
            sweep, // sweep top element from var table
            sweepn, // sweep n elements from var table
            jump, // jump to specific pc
            jumpoffset, // jump to offset pc
            localfget // get local funcion params
        }

        public enum Opexplicit : byte
        {
            // pencilbox additional operations
            extend = 30,
            scope,
            get,
            func,
            apply,
            print,

            // mathematical operations
            add,
            sub,
            div,
            mul,
            mod,

            // interactive with environment
            envGet,
            envSet,

            // flow control operations
            ifElse,

            // boolen operations
            eq,
            gt,
            ge,
            lt,
            le,

            // data struct constructor
            list,
            index,
            pop,
            push,
            shift,
            unshift,

            // ============ canvas 2d context properties =============
            canvas,
            fillStyle,
            font,
            globalAlpha,
            globalCompositeOperation,
            lineCap,
            lineDashOffset,
            lineJoin,
            lineWidth,
            miterLimit,
            shadowBlur,
            shadowColor,
            shadowOffsetX,
            shadowOffsetY,
            strokeStyle,
            textAlign,
            textBaseline,

            // ============= canvas 2d context operations ============
            arc,
            arcTo,
            beginPath,
            bezierCurveTo,
            clearRect,
            clip,
            closePath,
            createImageData,
            createLinearGradient,
            addColorStop,
            createPattern,
            createRadialGradient,
            drawImage,
            ellipse,
            fill,
            fillRect,
            fillText,
            getImageData,
            getLineDash,
            isPointInPath,
            isPointInStroke,
            lineTo,
            measureText,
            moveTo,
            putImageData,
            quadraticCurveTo,
            rect,
            restore,
            rotate,
            save,
            scale,
            setLineDash,
            setTransform,
            stroke,
            strokeRect,
            strokeText,
            transform,
            translate
        }
    }
}
