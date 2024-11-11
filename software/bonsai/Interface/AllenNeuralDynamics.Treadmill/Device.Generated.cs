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
            { 36, typeof(SensorDataDispatchRate) },
            { 37, typeof(BrakeCurrentSetPoint) },
            { 38, typeof(TareSensors) },
            { 39, typeof(ResetTareSensors) },
            { 40, typeof(EnableTorqueLimit) },
            { 41, typeof(TorqueLimitState) }
        };

        /// <summary>
        /// Gets the contents of the metadata file describing the <see cref="Treadmill"/>
        /// device registers.
        /// </summary>
        public static readonly string Metadata = GetDeviceMetadata();

        static string GetDeviceMetadata()
        {
            var deviceType = typeof(Device);
            using var metadataStream = deviceType.Assembly.GetManifestResourceStream($"{deviceType.Namespace}.device.yml");
            using var streamReader = new System.IO.StreamReader(metadataStream);
            return streamReader.ReadToEnd();
        }
    }

    /// <summary>
    /// Represents an operator that returns the contents of the metadata file
    /// describing the <see cref="Treadmill"/> device registers.
    /// </summary>
    [Description("Returns the contents of the metadata file describing the Treadmill device registers.")]
    public partial class GetMetadata : Source<string>
    {
        /// <summary>
        /// Returns an observable sequence with the contents of the metadata file
        /// describing the <see cref="Treadmill"/> device registers.
        /// </summary>
        /// <returns>
        /// A sequence with a single <see cref="string"/> object representing the
        /// contents of the metadata file.
        /// </returns>
        public override IObservable<string> Generate()
        {
            return Observable.Return(Device.Metadata);
        }
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
    /// <seealso cref="SensorDataDispatchRate"/>
    /// <seealso cref="BrakeCurrentSetPoint"/>
    /// <seealso cref="TareSensors"/>
    /// <seealso cref="ResetTareSensors"/>
    /// <seealso cref="EnableTorqueLimit"/>
    /// <seealso cref="TorqueLimitState"/>
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(Torque))]
    [XmlInclude(typeof(TorqueLoadCurrent))]
    [XmlInclude(typeof(SensorData))]
    [XmlInclude(typeof(SensorDataDispatchRate))]
    [XmlInclude(typeof(BrakeCurrentSetPoint))]
    [XmlInclude(typeof(TareSensors))]
    [XmlInclude(typeof(ResetTareSensors))]
    [XmlInclude(typeof(EnableTorqueLimit))]
    [XmlInclude(typeof(TorqueLimitState))]
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
    /// <seealso cref="SensorDataDispatchRate"/>
    /// <seealso cref="BrakeCurrentSetPoint"/>
    /// <seealso cref="TareSensors"/>
    /// <seealso cref="ResetTareSensors"/>
    /// <seealso cref="EnableTorqueLimit"/>
    /// <seealso cref="TorqueLimitState"/>
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(Torque))]
    [XmlInclude(typeof(TorqueLoadCurrent))]
    [XmlInclude(typeof(SensorData))]
    [XmlInclude(typeof(SensorDataDispatchRate))]
    [XmlInclude(typeof(BrakeCurrentSetPoint))]
    [XmlInclude(typeof(TareSensors))]
    [XmlInclude(typeof(ResetTareSensors))]
    [XmlInclude(typeof(EnableTorqueLimit))]
    [XmlInclude(typeof(TorqueLimitState))]
    [XmlInclude(typeof(TimestampedEncoder))]
    [XmlInclude(typeof(TimestampedTorque))]
    [XmlInclude(typeof(TimestampedTorqueLoadCurrent))]
    [XmlInclude(typeof(TimestampedSensorData))]
    [XmlInclude(typeof(TimestampedSensorDataDispatchRate))]
    [XmlInclude(typeof(TimestampedBrakeCurrentSetPoint))]
    [XmlInclude(typeof(TimestampedTareSensors))]
    [XmlInclude(typeof(TimestampedResetTareSensors))]
    [XmlInclude(typeof(TimestampedEnableTorqueLimit))]
    [XmlInclude(typeof(TimestampedTorqueLimitState))]
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
    /// <seealso cref="SensorDataDispatchRate"/>
    /// <seealso cref="BrakeCurrentSetPoint"/>
    /// <seealso cref="TareSensors"/>
    /// <seealso cref="ResetTareSensors"/>
    /// <seealso cref="EnableTorqueLimit"/>
    /// <seealso cref="TorqueLimitState"/>
    [XmlInclude(typeof(Encoder))]
    [XmlInclude(typeof(Torque))]
    [XmlInclude(typeof(TorqueLoadCurrent))]
    [XmlInclude(typeof(SensorData))]
    [XmlInclude(typeof(SensorDataDispatchRate))]
    [XmlInclude(typeof(BrakeCurrentSetPoint))]
    [XmlInclude(typeof(TareSensors))]
    [XmlInclude(typeof(ResetTareSensors))]
    [XmlInclude(typeof(EnableTorqueLimit))]
    [XmlInclude(typeof(TorqueLimitState))]
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
    /// Represents a register that contains the current accumulated number of ticks.
    /// </summary>
    [Description("Contains the current accumulated number of ticks.")]
    public partial class Encoder
    {
        /// <summary>
        /// Represents the address of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const int Address = 32;

        /// <summary>
        /// Represents the payload type of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.S32;

        /// <summary>
        /// Represents the length of the <see cref="Encoder"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Encoder"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static int GetPayload(HarpMessage message)
        {
            return message.GetPayloadInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Encoder"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<int> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadInt32();
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
        public static HarpMessage FromPayload(MessageType messageType, int value)
        {
            return HarpMessage.FromInt32(Address, messageType, value);
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
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, int value)
        {
            return HarpMessage.FromInt32(Address, timestamp, messageType, value);
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
        public static Timestamped<int> GetPayload(HarpMessage message)
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
        public const PayloadType RegisterType = PayloadType.S16;

        /// <summary>
        /// Represents the length of the <see cref="Torque"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="Torque"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static short GetPayload(HarpMessage message)
        {
            return message.GetPayloadInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="Torque"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<short> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadInt16();
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
        public static HarpMessage FromPayload(MessageType messageType, short value)
        {
            return HarpMessage.FromInt16(Address, messageType, value);
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
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, short value)
        {
            return HarpMessage.FromInt16(Address, timestamp, messageType, value);
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
        public static Timestamped<short> GetPayload(HarpMessage message)
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
        public const PayloadType RegisterType = PayloadType.S16;

        /// <summary>
        /// Represents the length of the <see cref="TorqueLoadCurrent"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TorqueLoadCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static short GetPayload(HarpMessage message)
        {
            return message.GetPayloadInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TorqueLoadCurrent"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<short> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadInt16();
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
        public static HarpMessage FromPayload(MessageType messageType, short value)
        {
            return HarpMessage.FromInt16(Address, messageType, value);
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
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, short value)
        {
            return HarpMessage.FromInt16(Address, timestamp, messageType, value);
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
        public static Timestamped<short> GetPayload(HarpMessage message)
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
        public const PayloadType RegisterType = PayloadType.S32;

        /// <summary>
        /// Represents the length of the <see cref="SensorData"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 3;

        static SensorDataPayload ParsePayload(int[] payload)
        {
            SensorDataPayload result;
            result.Encoder = payload[0];
            result.Torque = payload[1];
            result.TorqueLoadCurrent = payload[2];
            return result;
        }

        static int[] FormatPayload(SensorDataPayload value)
        {
            int[] result;
            result = new int[3];
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
            return ParsePayload(message.GetPayloadArray<int>());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SensorData"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<SensorDataPayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadArray<int>();
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
            return HarpMessage.FromInt32(Address, messageType, FormatPayload(value));
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
            return HarpMessage.FromInt32(Address, timestamp, messageType, FormatPayload(value));
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
    public partial class SensorDataDispatchRate
    {
        /// <summary>
        /// Represents the address of the <see cref="SensorDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int Address = 36;

        /// <summary>
        /// Represents the payload type of the <see cref="SensorDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SensorDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SensorDataDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SensorDataDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SensorDataDispatchRate"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SensorDataDispatchRate"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SensorDataDispatchRate"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SensorDataDispatchRate"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SensorDataDispatchRate register.
    /// </summary>
    /// <seealso cref="SensorDataDispatchRate"/>
    [Description("Filters and selects timestamped messages from the SensorDataDispatchRate register.")]
    public partial class TimestampedSensorDataDispatchRate
    {
        /// <summary>
        /// Represents the address of the <see cref="SensorDataDispatchRate"/> register. This field is constant.
        /// </summary>
        public const int Address = SensorDataDispatchRate.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SensorDataDispatchRate"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SensorDataDispatchRate.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.
    /// </summary>
    [Description("Sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.")]
    public partial class BrakeCurrentSetPoint
    {
        /// <summary>
        /// Represents the address of the <see cref="BrakeCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const int Address = 37;

        /// <summary>
        /// Represents the payload type of the <see cref="BrakeCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="BrakeCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="BrakeCurrentSetPoint"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="BrakeCurrentSetPoint"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="BrakeCurrentSetPoint"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="BrakeCurrentSetPoint"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="BrakeCurrentSetPoint"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="BrakeCurrentSetPoint"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// BrakeCurrentSetPoint register.
    /// </summary>
    /// <seealso cref="BrakeCurrentSetPoint"/>
    [Description("Filters and selects timestamped messages from the BrakeCurrentSetPoint register.")]
    public partial class TimestampedBrakeCurrentSetPoint
    {
        /// <summary>
        /// Represents the address of the <see cref="BrakeCurrentSetPoint"/> register. This field is constant.
        /// </summary>
        public const int Address = BrakeCurrentSetPoint.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="BrakeCurrentSetPoint"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return BrakeCurrentSetPoint.GetTimestampedPayload(message);
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
        public const PayloadType RegisterType = PayloadType.U8;

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
            return (Sensors)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Sensors> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
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
            return HarpMessage.FromByte(Address, messageType, (byte)value);
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
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
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
    /// Represents a register that removes the tare from the specified sensors.
    /// </summary>
    [Description("Removes the tare from the specified sensors.")]
    public partial class ResetTareSensors
    {
        /// <summary>
        /// Represents the address of the <see cref="ResetTareSensors"/> register. This field is constant.
        /// </summary>
        public const int Address = 39;

        /// <summary>
        /// Represents the payload type of the <see cref="ResetTareSensors"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ResetTareSensors"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ResetTareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static Sensors GetPayload(HarpMessage message)
        {
            return (Sensors)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ResetTareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Sensors> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((Sensors)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ResetTareSensors"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ResetTareSensors"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, Sensors value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ResetTareSensors"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ResetTareSensors"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, Sensors value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ResetTareSensors register.
    /// </summary>
    /// <seealso cref="ResetTareSensors"/>
    [Description("Filters and selects timestamped messages from the ResetTareSensors register.")]
    public partial class TimestampedResetTareSensors
    {
        /// <summary>
        /// Represents the address of the <see cref="ResetTareSensors"/> register. This field is constant.
        /// </summary>
        public const int Address = ResetTareSensors.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ResetTareSensors"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<Sensors> GetPayload(HarpMessage message)
        {
            return ResetTareSensors.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.
    /// </summary>
    [Description("Enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.")]
    public partial class EnableTorqueLimit
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableTorqueLimit"/> register. This field is constant.
        /// </summary>
        public const int Address = 40;

        /// <summary>
        /// Represents the payload type of the <see cref="EnableTorqueLimit"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="EnableTorqueLimit"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="EnableTorqueLimit"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static EnableFlag GetPayload(HarpMessage message)
        {
            return (EnableFlag)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="EnableTorqueLimit"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<EnableFlag> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((EnableFlag)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="EnableTorqueLimit"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableTorqueLimit"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, EnableFlag value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="EnableTorqueLimit"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="EnableTorqueLimit"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, EnableFlag value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// EnableTorqueLimit register.
    /// </summary>
    /// <seealso cref="EnableTorqueLimit"/>
    [Description("Filters and selects timestamped messages from the EnableTorqueLimit register.")]
    public partial class TimestampedEnableTorqueLimit
    {
        /// <summary>
        /// Represents the address of the <see cref="EnableTorqueLimit"/> register. This field is constant.
        /// </summary>
        public const int Address = EnableTorqueLimit.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="EnableTorqueLimit"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<EnableFlag> GetPayload(HarpMessage message)
        {
            return EnableTorqueLimit.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.
    /// </summary>
    [Description("A value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.")]
    public partial class TorqueLimitState
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueLimitState"/> register. This field is constant.
        /// </summary>
        public const int Address = 41;

        /// <summary>
        /// Represents the payload type of the <see cref="TorqueLimitState"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="TorqueLimitState"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="TorqueLimitState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TorqueLimitState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TorqueLimitState"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueLimitState"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TorqueLimitState"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TorqueLimitState"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TorqueLimitState register.
    /// </summary>
    /// <seealso cref="TorqueLimitState"/>
    [Description("Filters and selects timestamped messages from the TorqueLimitState register.")]
    public partial class TimestampedTorqueLimitState
    {
        /// <summary>
        /// Represents the address of the <see cref="TorqueLimitState"/> register. This field is constant.
        /// </summary>
        public const int Address = TorqueLimitState.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TorqueLimitState"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return TorqueLimitState.GetTimestampedPayload(message);
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
    /// <seealso cref="CreateSensorDataDispatchRatePayload"/>
    /// <seealso cref="CreateBrakeCurrentSetPointPayload"/>
    /// <seealso cref="CreateTareSensorsPayload"/>
    /// <seealso cref="CreateResetTareSensorsPayload"/>
    /// <seealso cref="CreateEnableTorqueLimitPayload"/>
    /// <seealso cref="CreateTorqueLimitStatePayload"/>
    [XmlInclude(typeof(CreateEncoderPayload))]
    [XmlInclude(typeof(CreateTorquePayload))]
    [XmlInclude(typeof(CreateTorqueLoadCurrentPayload))]
    [XmlInclude(typeof(CreateSensorDataPayload))]
    [XmlInclude(typeof(CreateSensorDataDispatchRatePayload))]
    [XmlInclude(typeof(CreateBrakeCurrentSetPointPayload))]
    [XmlInclude(typeof(CreateTareSensorsPayload))]
    [XmlInclude(typeof(CreateResetTareSensorsPayload))]
    [XmlInclude(typeof(CreateEnableTorqueLimitPayload))]
    [XmlInclude(typeof(CreateTorqueLimitStatePayload))]
    [XmlInclude(typeof(CreateTimestampedEncoderPayload))]
    [XmlInclude(typeof(CreateTimestampedTorquePayload))]
    [XmlInclude(typeof(CreateTimestampedTorqueLoadCurrentPayload))]
    [XmlInclude(typeof(CreateTimestampedSensorDataPayload))]
    [XmlInclude(typeof(CreateTimestampedSensorDataDispatchRatePayload))]
    [XmlInclude(typeof(CreateTimestampedBrakeCurrentSetPointPayload))]
    [XmlInclude(typeof(CreateTimestampedTareSensorsPayload))]
    [XmlInclude(typeof(CreateTimestampedResetTareSensorsPayload))]
    [XmlInclude(typeof(CreateTimestampedEnableTorqueLimitPayload))]
    [XmlInclude(typeof(CreateTimestampedTorqueLimitStatePayload))]
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
    /// that contains the current accumulated number of ticks.
    /// </summary>
    [DisplayName("EncoderPayload")]
    [Description("Creates a message payload that contains the current accumulated number of ticks.")]
    public partial class CreateEncoderPayload
    {
        /// <summary>
        /// Gets or sets the value that contains the current accumulated number of ticks.
        /// </summary>
        [Description("The value that contains the current accumulated number of ticks.")]
        public int Encoder { get; set; }

        /// <summary>
        /// Creates a message payload for the Encoder register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public int GetPayload()
        {
            return Encoder;
        }

        /// <summary>
        /// Creates a message that contains the current accumulated number of ticks.
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
    /// that contains the current accumulated number of ticks.
    /// </summary>
    [DisplayName("TimestampedEncoderPayload")]
    [Description("Creates a timestamped message payload that contains the current accumulated number of ticks.")]
    public partial class CreateTimestampedEncoderPayload : CreateEncoderPayload
    {
        /// <summary>
        /// Creates a timestamped message that contains the current accumulated number of ticks.
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
        public short Torque { get; set; }

        /// <summary>
        /// Creates a message payload for the Torque register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public short GetPayload()
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
        public short TorqueLoadCurrent { get; set; }

        /// <summary>
        /// Creates a message payload for the TorqueLoadCurrent register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public short GetPayload()
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
        public int Encoder { get; set; }

        /// <summary>
        /// Gets or sets a value to write on payload member Torque.
        /// </summary>
        [Description("")]
        public int Torque { get; set; }

        /// <summary>
        /// Gets or sets a value to write on payload member TorqueLoadCurrent.
        /// </summary>
        [Description("")]
        public int TorqueLoadCurrent { get; set; }

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
    [DisplayName("SensorDataDispatchRatePayload")]
    [Description("Creates a message payload that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
    public partial class CreateSensorDataDispatchRatePayload
    {
        /// <summary>
        /// Gets or sets the value that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
        /// </summary>
        [Range(min: 0, max: 1000)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
        public ushort SensorDataDispatchRate { get; set; } = 0;

        /// <summary>
        /// Creates a message payload for the SensorDataDispatchRate register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SensorDataDispatchRate;
        }

        /// <summary>
        /// Creates a message that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SensorDataDispatchRate register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SensorDataDispatchRate.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
    /// </summary>
    [DisplayName("TimestampedSensorDataDispatchRatePayload")]
    [Description("Creates a timestamped message payload that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).")]
    public partial class CreateTimestampedSensorDataDispatchRatePayload : CreateSensorDataDispatchRatePayload
    {
        /// <summary>
        /// Creates a timestamped message that value greater than 0 will enable the periodic dispatch of treadmill data events at the specified rate (sp/s).
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SensorDataDispatchRate register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.SensorDataDispatchRate.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.
    /// </summary>
    [DisplayName("BrakeCurrentSetPointPayload")]
    [Description("Creates a message payload that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.")]
    public partial class CreateBrakeCurrentSetPointPayload
    {
        /// <summary>
        /// Gets or sets the value that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.
        /// </summary>
        [Range(min: 0, max: 65535)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The value that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.")]
        public ushort BrakeCurrentSetPoint { get; set; } = 0;

        /// <summary>
        /// Creates a message payload for the BrakeCurrentSetPoint register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return BrakeCurrentSetPoint;
        }

        /// <summary>
        /// Creates a message that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the BrakeCurrentSetPoint register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.BrakeCurrentSetPoint.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.
    /// </summary>
    [DisplayName("TimestampedBrakeCurrentSetPointPayload")]
    [Description("Creates a timestamped message payload that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.")]
    public partial class CreateTimestampedBrakeCurrentSetPointPayload : CreateBrakeCurrentSetPointPayload
    {
        /// <summary>
        /// Creates a timestamped message that sets the raw value of the torque set-point to be applied to the treadmill. This value is cleared to 0 if torque_limiting is enabled and triggered. Further writes in this condition return a WRITE_ERROR.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the BrakeCurrentSetPoint register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.BrakeCurrentSetPoint.FromPayload(timestamp, messageType, GetPayload());
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
    /// Represents an operator that creates a message payload
    /// that removes the tare from the specified sensors.
    /// </summary>
    [DisplayName("ResetTareSensorsPayload")]
    [Description("Creates a message payload that removes the tare from the specified sensors.")]
    public partial class CreateResetTareSensorsPayload
    {
        /// <summary>
        /// Gets or sets the value that removes the tare from the specified sensors.
        /// </summary>
        [Description("The value that removes the tare from the specified sensors.")]
        public Sensors ResetTareSensors { get; set; }

        /// <summary>
        /// Creates a message payload for the ResetTareSensors register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public Sensors GetPayload()
        {
            return ResetTareSensors;
        }

        /// <summary>
        /// Creates a message that removes the tare from the specified sensors.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ResetTareSensors register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.ResetTareSensors.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that removes the tare from the specified sensors.
    /// </summary>
    [DisplayName("TimestampedResetTareSensorsPayload")]
    [Description("Creates a timestamped message payload that removes the tare from the specified sensors.")]
    public partial class CreateTimestampedResetTareSensorsPayload : CreateResetTareSensorsPayload
    {
        /// <summary>
        /// Creates a timestamped message that removes the tare from the specified sensors.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ResetTareSensors register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.ResetTareSensors.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.
    /// </summary>
    [DisplayName("EnableTorqueLimitPayload")]
    [Description("Creates a message payload that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.")]
    public partial class CreateEnableTorqueLimitPayload
    {
        /// <summary>
        /// Gets or sets the value that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.
        /// </summary>
        [Description("The value that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.")]
        public EnableFlag EnableTorqueLimit { get; set; }

        /// <summary>
        /// Creates a message payload for the EnableTorqueLimit register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public EnableFlag GetPayload()
        {
            return EnableTorqueLimit;
        }

        /// <summary>
        /// Creates a message that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the EnableTorqueLimit register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.EnableTorqueLimit.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.
    /// </summary>
    [DisplayName("TimestampedEnableTorqueLimitPayload")]
    [Description("Creates a timestamped message payload that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.")]
    public partial class CreateTimestampedEnableTorqueLimitPayload : CreateEnableTorqueLimitPayload
    {
        /// <summary>
        /// Creates a timestamped message that enables(1)/Disables(0) the brake if the maximum torque sensor value is detected. This register will be enabled by default.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the EnableTorqueLimit register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.EnableTorqueLimit.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.
    /// </summary>
    [DisplayName("TorqueLimitStatePayload")]
    [Description("Creates a message payload that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.")]
    public partial class CreateTorqueLimitStatePayload
    {
        /// <summary>
        /// Gets or sets the value that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.
        /// </summary>
        [Description("The value that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.")]
        public byte TorqueLimitState { get; set; }

        /// <summary>
        /// Creates a message payload for the TorqueLimitState register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return TorqueLimitState;
        }

        /// <summary>
        /// Creates a message that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TorqueLimitState register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueLimitState.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.
    /// </summary>
    [DisplayName("TimestampedTorqueLimitStatePayload")]
    [Description("Creates a timestamped message payload that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.")]
    public partial class CreateTimestampedTorqueLimitStatePayload : CreateTorqueLimitStatePayload
    {
        /// <summary>
        /// Creates a timestamped message that a value greater than 1 indicates that the torque limit has been triggered and the brake setpoint will be cleared. Writing a value of 0 will clear the torque limit state and re-enable the brake.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TorqueLimitState register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return AllenNeuralDynamics.Treadmill.TorqueLimitState.FromPayload(timestamp, messageType, GetPayload());
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
            int encoder,
            int torque,
            int torqueLoadCurrent)
        {
            Encoder = encoder;
            Torque = torque;
            TorqueLoadCurrent = torqueLoadCurrent;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Encoder;

        /// <summary>
        /// 
        /// </summary>
        public int Torque;

        /// <summary>
        /// 
        /// </summary>
        public int TorqueLoadCurrent;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the payload of
        /// the SensorData register.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the payload of the
        /// SensorData register.
        /// </returns>
        public override string ToString()
        {
            return "SensorDataPayload { " +
                "Encoder = " + Encoder + ", " +
                "Torque = " + Torque + ", " +
                "TorqueLoadCurrent = " + TorqueLoadCurrent + " " +
            "}";
        }
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
        BrakeCurrent = 0x4
    }
}
