namespace DevTools.IdGeneratorCommand.Services;

/// <summary>
/// Generates Twitter-style Snowflake ids => | 41 bits: timestamp | 10 bits: machine id | 12 bits: sequence |
/// - 41 bits – milliseconds elapsed since a custom epoch (enough for ~69 years).
/// - 10 bits – machine/worker id(0–1023), so multiple instances generating IDs don't collide.
/// - 12 bits – sequence number(0–4095), disambiguating multiple IDs generated within the same millisecond.
/// </summary>
public class SnowflakeIdGenerator(long machineId = 1)
{
    private const long Epoch = 1288834974657L; // Twitter Snowflake epoch: 2010-11-04T01:42:54.657Z
    private const int MachineIdBits = 10;
    private const int SequenceBits = 12;
    private const long MaxMachineId = -1L ^ (-1L << MachineIdBits); // Take all 1-bits, shift some of them out of the low end, then XOR against the original to keep only the bits that got shifted out — turning them all into a solid block of 1s at the bottom.
    private const long MaxSequence = -1L ^ (-1L << SequenceBits);
    private const int MachineIdShift = SequenceBits;
    private const int TimestampShift = MachineIdBits + SequenceBits;

    private readonly long _machineId = machineId is >= 0 and <= MaxMachineId
        ? machineId
        : throw new ArgumentOutOfRangeException(nameof(machineId), $"Machine id must be between 0 and {MaxMachineId}.");

    private readonly Lock _lock = new();
    private long _lastTimestamp = -1L;
    private long _sequence;

    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = CurrentTimestamp();

            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException("Clock moved backwards. Refusing to generate id.");
            }

            if (timestamp == _lastTimestamp)
            {
                // Wraps 0..4095 via bitmask (cheaper than %4096); wrapping to 0 means this millisecond's ids are exhausted.
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
                    timestamp = WaitForNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Epoch) << TimestampShift) | (_machineId << MachineIdShift) | _sequence;
        }
    }

    private static long CurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private static long WaitForNextMillis(long lastTimestamp)
    {
        long timestamp;
        do
        {
            timestamp = CurrentTimestamp();
        } while (timestamp <= lastTimestamp);

        return timestamp;
    }
}
