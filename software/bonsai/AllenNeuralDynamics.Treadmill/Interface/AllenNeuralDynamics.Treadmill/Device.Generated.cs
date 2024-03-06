using Bonsai;
using Bonsai.Harp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace AllenNeuralDynamics.Treadmill
{
    /// <summary>
    /// Generates events and processes commands for the Treadmill device connected
    /// at the specified serial port.
    /// </summary>
    [Combinator(MethodName = nameof(Generate))]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Generates events and processes commands for the Treadmill device.")]
    public partial class Device : Bonsai.Harp.Device, INamedElement
    {
        /// <summary>
        /// Represents the unique identity class of the <see cref="Treadmill"/> device.
        /// This field is constant.
        /// </summary>
        public const int WhoAmI = 1402;

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        public Device() : base(WhoAmI) { }

        string INamedElement.Name => nameof(Treadmill);

        /// <summary>
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static new IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
            (Bonsai.Harp.Device.RegisterMap.ToDictionary(entry => entry.Key, entry => entry.Value))
        {
            { 32, typeof(Encoder) },
            { 33, typeof(Torque) },
            { 34, typeof(TorqueLoadCurrent) },
            { 35, typeof(SensorData) },
            { 36, typeof(TreadmillDataDispatchRate) },
            { 37, typeof(BreakCurrentSetPoint) },
            { 38, typeof(TareSensors) }
        };
    }

    /// <summary>
    /// Represents an operator that groups the sequence of <see cref="Treadmill"/>" messages by register type.
    /// </summary>
    [Description("Groups the sequence of Treadmill messages by register type.")]
    public partial class GroupByRegister : Combinator<HarpMessage, IGroupedObservable<Type, HarpMessage>>
    {
        /// <summary>
        /// Groups an observable sequence of <see cref="Treadmill"/> messages
        /// by register type.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of observable groups, each of which corresponds to a unique
        /// <see cref="Treadmill"/> register.
        /// </returns>
        public override IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.GroupBy(message => Device.RegisterMap[message.Address]);
        }
    }

    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the <see cref="Treadmill"/> device.
    /// </summary>
    /// <seealso cref="Encoder"/>
    /// <seealso cref="Torque"/>
    /// <seealso cref="TorqueLoadCurrent"/>
    /// <seealso cref="SensorData"/>
    /// <seealso cref="TreadmillDataDispatchRate"/>
    /// <seealso cref="BreakCurrentSetPoint"/>
    /// <seealso cref="TareSensors"/>
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(Torque))]
    [XmlInclude(typeof(TorqueLoadCurrent))]
    [XmlInclude(typeof(SensorData))]
    [XmlInclude(typeof(TreadmillDataDispatchRate))]
    [XmlInclude(typeof(BreakCurrentSetPoint))]
    [XmlInclude(typeof(TareSensors))]
    [Description("Filters register-specific messages reported by the Treadmill device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new Encoder();
        }

        string INamedElement.Name
        {
            get => $"{nameof(Treadmill)}.{GetElementDisplayName(Register)}";
        }
    }

    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the Treadmill device.
    /// </summary>
    /// <seealso cref="Encoder"/>
    /// <seealso cref="Torque"/>
    /// <seealso cref="TorqueLoadCurrent"/>
    /// <seealso cref="SensorData"/>
    /// <seealso cref="TreadmillDataDispatchRate"/>
    /// <seealso cref="BreakCurrentSetPoint"/>
    /// <seealso cref="TareSensors"/>
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(Torque))]
    [XmlInclude(typeof(TorqueLoadCurrent))]
    [XmlInclude(typeof(SensorData))]
    [XmlInclude(typeof(TreadmillDataDispatchRate))]
    [XmlInclude(typeof(BreakCurrentSetPoint))]
    [XmlInclude(typeof(TareSensors))]
    [XmlInclude(typeof(TimestampedEncoder))]
    [XmlInclude(typeof(TimestampedTorque))]
    [XmlInclude(typeof(TimestampedTorqueLoadCurrent))]
    [XmlInclude(typeof(TimestampedSensorData))]
    [XmlInclude(typeof(TimestampedTreadmillDataDispatchRate))]
    [XmlInclude(typeof(TimestampedBreakCurrentSetPoint))]
    [XmlInclude(typeof(TimestampedTareSensors))]
    [Description("Filters and selects specific messages reported by the Treadmill device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new Encoder();
        }

        string INamedElement.Name => $"{nameof(Treadmill)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// Treadmill register messages.
    /// </summary>
    /// <seealso cref="Encoder"/>
    /// <seealso cref="Torque"/>
    /// <seealso cref="TorqueLoadCurrent"/>
    /// <seealso cref="SensorData"/>
    /// <seealso cref="TreadmillDataDispatchRate"/>
    /// <seealso cref="BreakCurrentSetPoint"/>
    /// <seealso cref="TareSensors"/>
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(Torque))]
    [XmlInclude(typeof(TorqueLoadCurrent))]
    [XmlInclude(typeof(SensorData))]
    [XmlInclude(typeof(TreadmillDataDispatchRate))]
    [XmlInclude(typeof(BreakCurrentSetPoint))]
    [XmlInclude(typeof(TareSensors))]
    [Description("Formats a sequence of values as specific Treadmill register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new Encoder();
        }

        string INamedElement.Name => $"{nameof(Treadmill)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that contains the current accumulated number of ticks. Overflows at 2^32.
    /// </summary>
    [Description("Contains the current accumulated number of ticks. Overflows at 2^32.")]
    public partial class Encoder
    {
        /// <summary>
        /// Represents the address of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Encoder"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Encoder"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Encoder"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Encoder"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Encoder"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Encoder"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Encoder register.
    /// </summary>
    /// <seealso cref="Encoder"/>
    [Description("Filters and selects timestamped messages from the Encoder register.")]
    public partial class TimestampedEncoder
    {
        /// <summary>
        /// Represents the address of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const int Address = Encoder.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Encoder"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return Encoder.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that contains the current torque value.
    /// </summary>
    [Description("Contains the current torque value.")]
    public partial class Torque
    {
        /// <summary>
        /// Represents the address of the <see cref="Torque"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

        /// <summary>
        /// Represents the payload type of the <see cref="Torque"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="Torque"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Torque"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Torque"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="Torque"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Torque"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="Torque"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="Torque"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// Torque register.
    /// </summary>
    /// <seealso cref="Torque"/>
    [Description("Filters and selects timestamped messages from the Torque register.")]
    public partial class TimestampedTorque
    {
        /// <summary>
        /// Represents the address of the <see cref="Torque"/> register. This field is constant.
        /// </summary>
        public const int Address = Torque.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="Torque"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return Torque.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that contains the current output current applied to the variable torque load.
    /// </summary>
    [Description("Contains the current output current applied to the variable torque load.")]
    public partial class TorqueLoadCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueLoadCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="TorqueLoadCurrent"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="TorqueLoadCurrent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TorqueLoadCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TorqueLoadCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TorqueLoadCurrent"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueLoadCurrent"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TorqueLoadCurrent"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueLoadCurrent"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TorqueLoadCurrent register.
    /// </summary>
    /// <seealso cref="TorqueLoadCurrent"/>
    [Description("Filters and selects timestamped messages from the TorqueLoadCurrent register.")]
    public partial class TimestampedTorqueLoadCurrent
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueLoadCurrent"/> register. This field is constant.
        /// </summary>
        public const int Address = TorqueLoadCurrent.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TorqueLoadCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return TorqueLoadCurrent.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].
    /// </summary>
    [Description("Emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent]")]
    public partial class SensorData
    {
        /// <summary>
        /// Represents the address of the <see cref="SensorData"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="SensorData"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="SensorData"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 3;

        static SensorDataPayload ParsePayload(uint[] payload)
        {
            SensorDataPayload result;
            result.Encoder = payload[0];
            result.Torque = payload[1];
            result.TorqueLoadCurrent = payload[2];
            return result;
        }

        static uint[] FormatPayload(SensorDataPayload value)
        {
            uint[] result;
            result = new uint[3];
            result[0] = value.Encoder;
            result[1] = value.Torque;
            result[2] = value.TorqueLoadCurrent;
            return result;
        }

        /// <summary>
        /// Returns the payload data for <see cref="SensorData"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static SensorDataPayload GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayloadArray<uint>());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SensorData"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<SensorDataPayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadArray<uint>();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SensorData"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SensorData"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, SensorDataPayload value)
        {
            return HarpMessage.FromUInt32(Address, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SensorData"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SensorData"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, SensorDataPayload value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, FormatPayload(value));
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SensorData register.
    /// </summary>
    /// <seealso cref="SensorData"/>
    [Description("Filters and selects timestamped messages from the SensorData register.")]
    public partial class TimestampedSensorData
    {
        /// <summary>
        /// Represents the address of the <see cref="SensorData"/> register. This field is constant.
        /// </summary>
        public const int Address = SensorData.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SensorData"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<SensorDataPayload> GetPayload(HarpMessage message)
        {
            return SensorData.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
    /// </summary>
    [Description("Value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
    public partial class TreadmillDataDispatchRate
    {
        /// <summary>
        /// Represents the address of the <see cref="TreadmillDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="TreadmillDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="TreadmillDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TreadmillDataDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TreadmillDataDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TreadmillDataDispatchRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TreadmillDataDispatchRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TreadmillDataDispatchRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TreadmillDataDispatchRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TreadmillDataDispatchRate register.
    /// </summary>
    /// <seealso cref="TreadmillDataDispatchRate"/>
    [Description("Filters and selects timestamped messages from the TreadmillDataDispatchRate register.")]
    public partial class TimestampedTreadmillDataDispatchRate
    {
        /// <summary>
        /// Represents the address of the <see cref="TreadmillDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int Address = TreadmillDataDispatchRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TreadmillDataDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return TreadmillDataDispatchRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that sets the raw value of the torque set-point to be applied to the treadmill.
    /// </summary>
    [Description("Sets the raw value of the torque set-point to be applied to the treadmill.")]
    public partial class BreakCurrentSetPoint
    {
        /// <summary>
        /// Represents the address of the <see cref="BreakCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="BreakCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="BreakCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="BreakCurrentSetPoint"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="BreakCurrentSetPoint"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="BreakCurrentSetPoint"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="BreakCurrentSetPoint"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="BreakCurrentSetPoint"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="BreakCurrentSetPoint"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// BreakCurrentSetPoint register.
    /// </summary>
    /// <seealso cref="BreakCurrentSetPoint"/>
    [Description("Filters and selects timestamped messages from the BreakCurrentSetPoint register.")]
    public partial class TimestampedBreakCurrentSetPoint
    {
        /// <summary>
        /// Represents the address of the <see cref="BreakCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const int Address = BreakCurrentSetPoint.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="BreakCurrentSetPoint"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return BreakCurrentSetPoint.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that tares the specified sensors.
    /// </summary>
    [Description("Tares the specified sensors.")]
    public partial class TareSensors
    {
        /// <summary>
        /// Represents the address of the <see cref="TareSensors"/> register. This field is constant.
        /// </summary>
        public const int Address = 38;

        /// <summary>
        /// Represents the payload type of the <see cref="TareSensors"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="TareSensors"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Sensors GetPayload(HarpMessage message)
        {
            return (Sensors)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Sensors> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((Sensors)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TareSensors"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TareSensors"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Sensors value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TareSensors"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TareSensors"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Sensors value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TareSensors register.
    /// </summary>
    /// <seealso cref="TareSensors"/>
    [Description("Filters and selects timestamped messages from the TareSensors register.")]
    public partial class TimestampedTareSensors
    {
        /// <summary>
        /// Represents the address of the <see cref="TareSensors"/> register. This field is constant.
        /// </summary>
        public const int Address = TareSensors.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Sensors> GetPayload(HarpMessage message)
        {
            return TareSensors.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// Treadmill device.
    /// </summary>
    /// <seealso cref="CreateEncoderPayload"/>
    /// <seealso cref="CreateTorquePayload"/>
    /// <seealso cref="CreateTorqueLoadCurrentPayload"/>
    /// <seealso cref="CreateSensorDataPayload"/>
    /// <seealso cref="CreateTreadmillDataDispatchRatePayload"/>
    /// <seealso cref="CreateBreakCurrentSetPointPayload"/>
    /// <seealso cref="CreateTareSensorsPayload"/>
    [XmlInclude(typeof(CreateEncoderPayload))]
    [XmlInclude(typeof(CreateTorquePayload))]
    [XmlInclude(typeof(CreateTorqueLoadCurrentPayload))]
    [XmlInclude(typeof(CreateSensorDataPayload))]
    [XmlInclude(typeof(CreateTreadmillDataDispatchRatePayload))]
    [XmlInclude(typeof(CreateBreakCurrentSetPointPayload))]
    [XmlInclude(typeof(CreateTareSensorsPayload))]
    [XmlInclude(typeof(CreateTimestampedEncoderPayload))]
    [XmlInclude(typeof(CreateTimestampedTorquePayload))]
    [XmlInclude(typeof(CreateTimestampedTorqueLoadCurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedSensorDataPayload))]
    [XmlInclude(typeof(CreateTimestampedTreadmillDataDispatchRatePayload))]
    [XmlInclude(typeof(CreateTimestampedBreakCurrentSetPointPayload))]
    [XmlInclude(typeof(CreateTimestampedTareSensorsPayload))]
    [Description("Creates standard message payloads for the Treadmill device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateEncoderPayload();
        }

        string INamedElement.Name => $"{nameof(Treadmill)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that contains the current accumulated number of ticks. Overflows at 2^32.
    /// </summary>
    [DisplayName("EncoderPayload")]
    [Description("Creates a message payload that contains the current accumulated number of ticks. Overflows at 2^32.")]
    public partial class CreateEncoderPayload
    {
        /// <summary>
        /// Gets or sets the value that contains the current accumulated number of ticks. Overflows at 2^32.
        /// </summary>
        [Description("The value that contains the current accumulated number of ticks. Overflows at 2^32.")]
        public uint Encoder { get; set; }

        /// <summary>
        /// Creates a message payload for the Encoder register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return Encoder;
        }

        /// <summary>
        /// Creates a message that contains the current accumulated number of ticks. Overflows at 2^32.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Encoder register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.Encoder.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that contains the current accumulated number of ticks. Overflows at 2^32.
    /// </summary>
    [DisplayName("TimestampedEncoderPayload")]
    [Description("Creates a timestamped message payload that contains the current accumulated number of ticks. Overflows at 2^32.")]
    public partial class CreateTimestampedEncoderPayload : CreateEncoderPayload
    {
        /// <summary>
        /// Creates a timestamped message that contains the current accumulated number of ticks. Overflows at 2^32.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Encoder register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.Encoder.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that contains the current torque value.
    /// </summary>
    [DisplayName("TorquePayload")]
    [Description("Creates a message payload that contains the current torque value.")]
    public partial class CreateTorquePayload
    {
        /// <summary>
        /// Gets or sets the value that contains the current torque value.
        /// </summary>
        [Description("The value that contains the current torque value.")]
        public ushort Torque { get; set; }

        /// <summary>
        /// Creates a message payload for the Torque register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return Torque;
        }

        /// <summary>
        /// Creates a message that contains the current torque value.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the Torque register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.Torque.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that contains the current torque value.
    /// </summary>
    [DisplayName("TimestampedTorquePayload")]
    [Description("Creates a timestamped message payload that contains the current torque value.")]
    public partial class CreateTimestampedTorquePayload : CreateTorquePayload
    {
        /// <summary>
        /// Creates a timestamped message that contains the current torque value.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the Torque register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.Torque.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that contains the current output current applied to the variable torque load.
    /// </summary>
    [DisplayName("TorqueLoadCurrentPayload")]
    [Description("Creates a message payload that contains the current output current applied to the variable torque load.")]
    public partial class CreateTorqueLoadCurrentPayload
    {
        /// <summary>
        /// Gets or sets the value that contains the current output current applied to the variable torque load.
        /// </summary>
        [Description("The value that contains the current output current applied to the variable torque load.")]
        public ushort TorqueLoadCurrent { get; set; }

        /// <summary>
        /// Creates a message payload for the TorqueLoadCurrent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return TorqueLoadCurrent;
        }

        /// <summary>
        /// Creates a message that contains the current output current applied to the variable torque load.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TorqueLoadCurrent register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueLoadCurrent.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that contains the current output current applied to the variable torque load.
    /// </summary>
    [DisplayName("TimestampedTorqueLoadCurrentPayload")]
    [Description("Creates a timestamped message payload that contains the current output current applied to the variable torque load.")]
    public partial class CreateTimestampedTorqueLoadCurrentPayload : CreateTorqueLoadCurrentPayload
    {
        /// <summary>
        /// Creates a timestamped message that contains the current output current applied to the variable torque load.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TorqueLoadCurrent register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueLoadCurrent.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].
    /// </summary>
    [DisplayName("SensorDataPayload")]
    [Description("Creates a message payload that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].")]
    public partial class CreateSensorDataPayload
    {
        /// <summary>
        /// Gets or sets a value to write on payload member Encoder.
        /// </summary>
        [Description("")]
        public uint Encoder { get; set; }

        /// <summary>
        /// Gets or sets a value to write on payload member Torque.
        /// </summary>
        [Description("")]
        public uint Torque { get; set; }

        /// <summary>
        /// Gets or sets a value to write on payload member TorqueLoadCurrent.
        /// </summary>
        [Description("")]
        public uint TorqueLoadCurrent { get; set; }

        /// <summary>
        /// Creates a message payload for the SensorData register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public SensorDataPayload GetPayload()
        {
            SensorDataPayload value;
            value.Encoder = Encoder;
            value.Torque = Torque;
            value.TorqueLoadCurrent = TorqueLoadCurrent;
            return value;
        }

        /// <summary>
        /// Creates a message that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SensorData register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SensorData.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].
    /// </summary>
    [DisplayName("TimestampedSensorDataPayload")]
    [Description("Creates a timestamped message payload that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].")]
    public partial class CreateTimestampedSensorDataPayload : CreateSensorDataPayload
    {
        /// <summary>
        /// Creates a timestamped message that emits a periodic event containing the packaged treadmill data. [Encoder, Torque, TorqueLoadCurrent].
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SensorData register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SensorData.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
    /// </summary>
    [DisplayName("TreadmillDataDispatchRatePayload")]
    [Description("Creates a message payload that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
    public partial class CreateTreadmillDataDispatchRatePayload
    {
        /// <summary>
        /// Gets or sets the value that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
        /// </summary>
        [Range(min: 0, max: 1000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
        public uint TreadmillDataDispatchRate { get; set; } = 0;

        /// <summary>
        /// Creates a message payload for the TreadmillDataDispatchRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return TreadmillDataDispatchRate;
        }

        /// <summary>
        /// Creates a message that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TreadmillDataDispatchRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TreadmillDataDispatchRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
    /// </summary>
    [DisplayName("TimestampedTreadmillDataDispatchRatePayload")]
    [Description("Creates a timestamped message payload that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
    public partial class CreateTimestampedTreadmillDataDispatchRatePayload : CreateTreadmillDataDispatchRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TreadmillDataDispatchRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TreadmillDataDispatchRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that sets the raw value of the torque set-point to be applied to the treadmill.
    /// </summary>
    [DisplayName("BreakCurrentSetPointPayload")]
    [Description("Creates a message payload that sets the raw value of the torque set-point to be applied to the treadmill.")]
    public partial class CreateBreakCurrentSetPointPayload
    {
        /// <summary>
        /// Gets or sets the value that sets the raw value of the torque set-point to be applied to the treadmill.
        /// </summary>
        [Description("The value that sets the raw value of the torque set-point to be applied to the treadmill.")]
        public ushort BreakCurrentSetPoint { get; set; }

        /// <summary>
        /// Creates a message payload for the BreakCurrentSetPoint register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return BreakCurrentSetPoint;
        }

        /// <summary>
        /// Creates a message that sets the raw value of the torque set-point to be applied to the treadmill.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the BreakCurrentSetPoint register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.BreakCurrentSetPoint.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that sets the raw value of the torque set-point to be applied to the treadmill.
    /// </summary>
    [DisplayName("TimestampedBreakCurrentSetPointPayload")]
    [Description("Creates a timestamped message payload that sets the raw value of the torque set-point to be applied to the treadmill.")]
    public partial class CreateTimestampedBreakCurrentSetPointPayload : CreateBreakCurrentSetPointPayload
    {
        /// <summary>
        /// Creates a timestamped message that sets the raw value of the torque set-point to be applied to the treadmill.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the BreakCurrentSetPoint register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.BreakCurrentSetPoint.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that tares the specified sensors.
    /// </summary>
    [DisplayName("TareSensorsPayload")]
    [Description("Creates a message payload that tares the specified sensors.")]
    public partial class CreateTareSensorsPayload
    {
        /// <summary>
        /// Gets or sets the value that tares the specified sensors.
        /// </summary>
        [Description("The value that tares the specified sensors.")]
        public Sensors TareSensors { get; set; }

        /// <summary>
        /// Creates a message payload for the TareSensors register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Sensors GetPayload()
        {
            return TareSensors;
        }

        /// <summary>
        /// Creates a message that tares the specified sensors.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TareSensors register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TareSensors.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that tares the specified sensors.
    /// </summary>
    [DisplayName("TimestampedTareSensorsPayload")]
    [Description("Creates a timestamped message payload that tares the specified sensors.")]
    public partial class CreateTimestampedTareSensorsPayload : CreateTareSensorsPayload
    {
        /// <summary>
        /// Creates a timestamped message that tares the specified sensors.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TareSensors register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TareSensors.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents the payload of the SensorData register.
    /// </summary>
    public struct SensorDataPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorDataPayload"/> structure.
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="torque"></param>
        /// <param name="torqueLoadCurrent"></param>
        public SensorDataPayload(
            uint encoder,
            uint torque,
            uint torqueLoadCurrent)
        {
            Encoder = encoder;
            Torque = torque;
            TorqueLoadCurrent = torqueLoadCurrent;
        }

        /// <summary>
        /// 
        /// </summary>
        public uint Encoder;

        /// <summary>
        /// 
        /// </summary>
        public uint Torque;

        /// <summary>
        /// 
        /// </summary>
        public uint TorqueLoadCurrent;
    }

    /// <summary>
    /// Available sensors.
    /// </summary>
    [Flags]
    public enum Sensors : byte
    {
        None = 0x0,
        Encoder = 0x1,
        Torque = 0x2,
        BreakCurrent = 0x4
    }
}
