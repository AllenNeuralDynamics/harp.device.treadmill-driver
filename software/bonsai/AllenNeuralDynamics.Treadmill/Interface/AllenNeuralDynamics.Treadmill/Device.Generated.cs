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
            { 32, typeof(TorqueRaw) },
            { 32, typeof(TorqueLoadCurrentRaw) },
            { 33, typeof(Encoder) },
            { 37, typeof(EncoderMode) },
            { 34, typeof(SetTorqueRaw) },
            { 35, typeof(SubsystemReset) }
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
    /// <seealso cref="TorqueRaw"/>
    /// <seealso cref="TorqueLoadCurrentRaw"/>
    /// <seealso cref="Encoder"/>
    /// <seealso cref="EncoderMode"/>
    /// <seealso cref="SetTorqueRaw"/>
    /// <seealso cref="SubsystemReset"/>
    [XmlInclude(typeof(TorqueRaw))]
    [XmlInclude(typeof(TorqueLoadCurrentRaw))]
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(EncoderMode))]
    [XmlInclude(typeof(SetTorqueRaw))]
    [XmlInclude(typeof(SubsystemReset))]
    [Description("Filters register-specific messages reported by the Treadmill device.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new TorqueRaw();
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
    /// <seealso cref="TorqueRaw"/>
    /// <seealso cref="TorqueLoadCurrentRaw"/>
    /// <seealso cref="Encoder"/>
    /// <seealso cref="EncoderMode"/>
    /// <seealso cref="SetTorqueRaw"/>
    /// <seealso cref="SubsystemReset"/>
    [XmlInclude(typeof(TorqueRaw))]
    [XmlInclude(typeof(TorqueLoadCurrentRaw))]
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(EncoderMode))]
    [XmlInclude(typeof(SetTorqueRaw))]
    [XmlInclude(typeof(SubsystemReset))]
    [XmlInclude(typeof(TimestampedTorqueRaw))]
    [XmlInclude(typeof(TimestampedTorqueLoadCurrentRaw))]
    [XmlInclude(typeof(TimestampedEncoder))]
    [XmlInclude(typeof(TimestampedEncoderMode))]
    [XmlInclude(typeof(TimestampedSetTorqueRaw))]
    [XmlInclude(typeof(TimestampedSubsystemReset))]
    [Description("Filters and selects specific messages reported by the Treadmill device.")]
    public partial class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new TorqueRaw();
        }

        string INamedElement.Name => $"{nameof(Treadmill)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// Treadmill register messages.
    /// </summary>
    /// <seealso cref="TorqueRaw"/>
    /// <seealso cref="TorqueLoadCurrentRaw"/>
    /// <seealso cref="Encoder"/>
    /// <seealso cref="EncoderMode"/>
    /// <seealso cref="SetTorqueRaw"/>
    /// <seealso cref="SubsystemReset"/>
    [XmlInclude(typeof(TorqueRaw))]
    [XmlInclude(typeof(TorqueLoadCurrentRaw))]
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(EncoderMode))]
    [XmlInclude(typeof(SetTorqueRaw))]
    [XmlInclude(typeof(SubsystemReset))]
    [Description("Formats a sequence of values as specific Treadmill register messages.")]
    public partial class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new TorqueRaw();
        }

        string INamedElement.Name => $"{nameof(Treadmill)}.{GetElementDisplayName(Register)}";
    }

    /// <summary>
    /// Represents a register that emits a periodic event containing a raw measurement value from the angular torque sensor.
    /// </summary>
    [Description("Emits a periodic event containing a raw measurement value from the angular torque sensor.")]
    public partial class TorqueRaw
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueRaw"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="TorqueRaw"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="TorqueRaw"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TorqueRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TorqueRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TorqueRaw"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueRaw"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TorqueRaw"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueRaw"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TorqueRaw register.
    /// </summary>
    /// <seealso cref="TorqueRaw"/>
    [Description("Filters and selects timestamped messages from the TorqueRaw register.")]
    public partial class TimestampedTorqueRaw
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueRaw"/> register. This field is constant.
        /// </summary>
        public const int Address = TorqueRaw.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TorqueRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return TorqueRaw.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.
    /// </summary>
    [Description("Emits a periodic event containing a raw measurement of the output current applied to the variable torque load.")]
    public partial class TorqueLoadCurrentRaw
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueLoadCurrentRaw"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="TorqueLoadCurrentRaw"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="TorqueLoadCurrentRaw"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TorqueLoadCurrentRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TorqueLoadCurrentRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TorqueLoadCurrentRaw"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueLoadCurrentRaw"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TorqueLoadCurrentRaw"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueLoadCurrentRaw"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TorqueLoadCurrentRaw register.
    /// </summary>
    /// <seealso cref="TorqueLoadCurrentRaw"/>
    [Description("Filters and selects timestamped messages from the TorqueLoadCurrentRaw register.")]
    public partial class TimestampedTorqueLoadCurrentRaw
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueLoadCurrentRaw"/> register. This field is constant.
        /// </summary>
        public const int Address = TorqueLoadCurrentRaw.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TorqueLoadCurrentRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return TorqueLoadCurrentRaw.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that emits a periodic event containing a raw measurement value from the treadmill encoder.
    /// </summary>
    [Description("Emits a periodic event containing a raw measurement value from the treadmill encoder.")]
    public partial class Encoder
    {
        /// <summary>
        /// Represents the address of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const int Address = 33;

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
    /// Represents a register that specifies the type of reading made from the encoder.
    /// </summary>
    [Description("Specifies the type of reading made from the encoder.")]
    public partial class EncoderMode
    {
        /// <summary>
        /// Represents the address of the <see cref="EncoderMode"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="EncoderMode"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EncoderMode"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EncoderMode"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static EncoderModeConfig GetPayload(HarpMessage message)
        {
            return (EncoderModeConfig)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EncoderMode"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<EncoderModeConfig> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((EncoderModeConfig)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EncoderMode"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EncoderMode"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, EncoderModeConfig value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EncoderMode"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EncoderMode"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, EncoderModeConfig value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EncoderMode register.
    /// </summary>
    /// <seealso cref="EncoderMode"/>
    [Description("Filters and selects timestamped messages from the EncoderMode register.")]
    public partial class TimestampedEncoderMode
    {
        /// <summary>
        /// Represents the address of the <see cref="EncoderMode"/> register. This field is constant.
        /// </summary>
        public const int Address = EncoderMode.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EncoderMode"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<EncoderModeConfig> GetPayload(HarpMessage message)
        {
            return EncoderMode.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that sets the raw value of the torque set-point to be applied to the treadmill.
    /// </summary>
    [Description("Sets the raw value of the torque set-point to be applied to the treadmill.")]
    public partial class SetTorqueRaw
    {
        /// <summary>
        /// Represents the address of the <see cref="SetTorqueRaw"/> register. This field is constant.
        /// </summary>
        public const int Address = 34;

        /// <summary>
        /// Represents the payload type of the <see cref="SetTorqueRaw"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="SetTorqueRaw"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SetTorqueRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SetTorqueRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SetTorqueRaw"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SetTorqueRaw"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SetTorqueRaw"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SetTorqueRaw"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SetTorqueRaw register.
    /// </summary>
    /// <seealso cref="SetTorqueRaw"/>
    [Description("Filters and selects timestamped messages from the SetTorqueRaw register.")]
    public partial class TimestampedSetTorqueRaw
    {
        /// <summary>
        /// Represents the address of the <see cref="SetTorqueRaw"/> register. This field is constant.
        /// </summary>
        public const int Address = SetTorqueRaw.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SetTorqueRaw"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return SetTorqueRaw.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that resets the specified treadmill subsystem.
    /// </summary>
    [Description("Resets the specified treadmill subsystem.")]
    public partial class SubsystemReset
    {
        /// <summary>
        /// Represents the address of the <see cref="SubsystemReset"/> register. This field is constant.
        /// </summary>
        public const int Address = 35;

        /// <summary>
        /// Represents the payload type of the <see cref="SubsystemReset"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="SubsystemReset"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SubsystemReset"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Subsystems GetPayload(HarpMessage message)
        {
            return (Subsystems)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SubsystemReset"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Subsystems> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Subsystems)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SubsystemReset"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SubsystemReset"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Subsystems value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SubsystemReset"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SubsystemReset"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Subsystems value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SubsystemReset register.
    /// </summary>
    /// <seealso cref="SubsystemReset"/>
    [Description("Filters and selects timestamped messages from the SubsystemReset register.")]
    public partial class TimestampedSubsystemReset
    {
        /// <summary>
        /// Represents the address of the <see cref="SubsystemReset"/> register. This field is constant.
        /// </summary>
        public const int Address = SubsystemReset.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SubsystemReset"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Subsystems> GetPayload(HarpMessage message)
        {
            return SubsystemReset.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator which creates standard message payloads for the
    /// Treadmill device.
    /// </summary>
    /// <seealso cref="CreateTorqueRawPayload"/>
    /// <seealso cref="CreateTorqueLoadCurrentRawPayload"/>
    /// <seealso cref="CreateEncoderPayload"/>
    /// <seealso cref="CreateEncoderModePayload"/>
    /// <seealso cref="CreateSetTorqueRawPayload"/>
    /// <seealso cref="CreateSubsystemResetPayload"/>
    [XmlInclude(typeof(CreateTorqueRawPayload))]
    [XmlInclude(typeof(CreateTorqueLoadCurrentRawPayload))]
    [XmlInclude(typeof(CreateEncoderPayload))]
    [XmlInclude(typeof(CreateEncoderModePayload))]
    [XmlInclude(typeof(CreateSetTorqueRawPayload))]
    [XmlInclude(typeof(CreateSubsystemResetPayload))]
    [XmlInclude(typeof(CreateTimestampedTorqueRawPayload))]
    [XmlInclude(typeof(CreateTimestampedTorqueLoadCurrentRawPayload))]
    [XmlInclude(typeof(CreateTimestampedEncoderPayload))]
    [XmlInclude(typeof(CreateTimestampedEncoderModePayload))]
    [XmlInclude(typeof(CreateTimestampedSetTorqueRawPayload))]
    [XmlInclude(typeof(CreateTimestampedSubsystemResetPayload))]
    [Description("Creates standard message payloads for the Treadmill device.")]
    public partial class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateTorqueRawPayload();
        }

        string INamedElement.Name => $"{nameof(Treadmill)}.{GetElementDisplayName(Payload)}";
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that emits a periodic event containing a raw measurement value from the angular torque sensor.
    /// </summary>
    [DisplayName("TorqueRawPayload")]
    [Description("Creates a message payload that emits a periodic event containing a raw measurement value from the angular torque sensor.")]
    public partial class CreateTorqueRawPayload
    {
        /// <summary>
        /// Gets or sets the value that emits a periodic event containing a raw measurement value from the angular torque sensor.
        /// </summary>
        [Description("The value that emits a periodic event containing a raw measurement value from the angular torque sensor.")]
        public ushort TorqueRaw { get; set; }

        /// <summary>
        /// Creates a message payload for the TorqueRaw register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return TorqueRaw;
        }

        /// <summary>
        /// Creates a message that emits a periodic event containing a raw measurement value from the angular torque sensor.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TorqueRaw register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueRaw.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that emits a periodic event containing a raw measurement value from the angular torque sensor.
    /// </summary>
    [DisplayName("TimestampedTorqueRawPayload")]
    [Description("Creates a timestamped message payload that emits a periodic event containing a raw measurement value from the angular torque sensor.")]
    public partial class CreateTimestampedTorqueRawPayload : CreateTorqueRawPayload
    {
        /// <summary>
        /// Creates a timestamped message that emits a periodic event containing a raw measurement value from the angular torque sensor.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TorqueRaw register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueRaw.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.
    /// </summary>
    [DisplayName("TorqueLoadCurrentRawPayload")]
    [Description("Creates a message payload that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.")]
    public partial class CreateTorqueLoadCurrentRawPayload
    {
        /// <summary>
        /// Gets or sets the value that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.
        /// </summary>
        [Description("The value that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.")]
        public ushort TorqueLoadCurrentRaw { get; set; }

        /// <summary>
        /// Creates a message payload for the TorqueLoadCurrentRaw register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return TorqueLoadCurrentRaw;
        }

        /// <summary>
        /// Creates a message that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TorqueLoadCurrentRaw register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueLoadCurrentRaw.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.
    /// </summary>
    [DisplayName("TimestampedTorqueLoadCurrentRawPayload")]
    [Description("Creates a timestamped message payload that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.")]
    public partial class CreateTimestampedTorqueLoadCurrentRawPayload : CreateTorqueLoadCurrentRawPayload
    {
        /// <summary>
        /// Creates a timestamped message that emits a periodic event containing a raw measurement of the output current applied to the variable torque load.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TorqueLoadCurrentRaw register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueLoadCurrentRaw.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that emits a periodic event containing a raw measurement value from the treadmill encoder.
    /// </summary>
    [DisplayName("EncoderPayload")]
    [Description("Creates a message payload that emits a periodic event containing a raw measurement value from the treadmill encoder.")]
    public partial class CreateEncoderPayload
    {
        /// <summary>
        /// Gets or sets the value that emits a periodic event containing a raw measurement value from the treadmill encoder.
        /// </summary>
        [Description("The value that emits a periodic event containing a raw measurement value from the treadmill encoder.")]
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
        /// Creates a message that emits a periodic event containing a raw measurement value from the treadmill encoder.
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
    /// that emits a periodic event containing a raw measurement value from the treadmill encoder.
    /// </summary>
    [DisplayName("TimestampedEncoderPayload")]
    [Description("Creates a timestamped message payload that emits a periodic event containing a raw measurement value from the treadmill encoder.")]
    public partial class CreateTimestampedEncoderPayload : CreateEncoderPayload
    {
        /// <summary>
        /// Creates a timestamped message that emits a periodic event containing a raw measurement value from the treadmill encoder.
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
    /// that specifies the type of reading made from the encoder.
    /// </summary>
    [DisplayName("EncoderModePayload")]
    [Description("Creates a message payload that specifies the type of reading made from the encoder.")]
    public partial class CreateEncoderModePayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the type of reading made from the encoder.
        /// </summary>
        [Description("The value that specifies the type of reading made from the encoder.")]
        public EncoderModeConfig EncoderMode { get; set; }

        /// <summary>
        /// Creates a message payload for the EncoderMode register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public EncoderModeConfig GetPayload()
        {
            return EncoderMode;
        }

        /// <summary>
        /// Creates a message that specifies the type of reading made from the encoder.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EncoderMode register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.EncoderMode.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the type of reading made from the encoder.
    /// </summary>
    [DisplayName("TimestampedEncoderModePayload")]
    [Description("Creates a timestamped message payload that specifies the type of reading made from the encoder.")]
    public partial class CreateTimestampedEncoderModePayload : CreateEncoderModePayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the type of reading made from the encoder.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EncoderMode register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.EncoderMode.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that sets the raw value of the torque set-point to be applied to the treadmill.
    /// </summary>
    [DisplayName("SetTorqueRawPayload")]
    [Description("Creates a message payload that sets the raw value of the torque set-point to be applied to the treadmill.")]
    public partial class CreateSetTorqueRawPayload
    {
        /// <summary>
        /// Gets or sets the value that sets the raw value of the torque set-point to be applied to the treadmill.
        /// </summary>
        [Description("The value that sets the raw value of the torque set-point to be applied to the treadmill.")]
        public uint SetTorqueRaw { get; set; }

        /// <summary>
        /// Creates a message payload for the SetTorqueRaw register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return SetTorqueRaw;
        }

        /// <summary>
        /// Creates a message that sets the raw value of the torque set-point to be applied to the treadmill.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SetTorqueRaw register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SetTorqueRaw.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that sets the raw value of the torque set-point to be applied to the treadmill.
    /// </summary>
    [DisplayName("TimestampedSetTorqueRawPayload")]
    [Description("Creates a timestamped message payload that sets the raw value of the torque set-point to be applied to the treadmill.")]
    public partial class CreateTimestampedSetTorqueRawPayload : CreateSetTorqueRawPayload
    {
        /// <summary>
        /// Creates a timestamped message that sets the raw value of the torque set-point to be applied to the treadmill.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SetTorqueRaw register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SetTorqueRaw.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that resets the specified treadmill subsystem.
    /// </summary>
    [DisplayName("SubsystemResetPayload")]
    [Description("Creates a message payload that resets the specified treadmill subsystem.")]
    public partial class CreateSubsystemResetPayload
    {
        /// <summary>
        /// Gets or sets the value that resets the specified treadmill subsystem.
        /// </summary>
        [Description("The value that resets the specified treadmill subsystem.")]
        public Subsystems SubsystemReset { get; set; }

        /// <summary>
        /// Creates a message payload for the SubsystemReset register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Subsystems GetPayload()
        {
            return SubsystemReset;
        }

        /// <summary>
        /// Creates a message that resets the specified treadmill subsystem.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SubsystemReset register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SubsystemReset.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that resets the specified treadmill subsystem.
    /// </summary>
    [DisplayName("TimestampedSubsystemResetPayload")]
    [Description("Creates a timestamped message payload that resets the specified treadmill subsystem.")]
    public partial class CreateTimestampedSubsystemResetPayload : CreateSubsystemResetPayload
    {
        /// <summary>
        /// Creates a timestamped message that resets the specified treadmill subsystem.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SubsystemReset register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SubsystemReset.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Specifies the subsystem to reset.
    /// </summary>
    [Flags]
    public enum Subsystems : byte
    {
        None = 0x0,
        Encoder = 0x1,
        Torque = 0x2
    }

    /// <summary>
    /// Specifies the type of reading made from the encoder.
    /// </summary>
    public enum EncoderModeConfig : byte
    {
        Position = 0,
        Displacement = 1
    }
}
