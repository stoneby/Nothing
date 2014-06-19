using System;
using System.Text;

namespace com.kx.sglm.core.model
{

	/// <summary>
	/// Simple implement of BitSet, refer to <seealso cref="BitSet"/>
	/// 
	/// @author fangyong
	/// 
	/// </summary>
	public class SimpleBitSet
	{
		private sbyte[] words;

		private const sbyte SETTED_VAL = 1;

		private const sbyte UNSETTED_VAL = 0;

		/// <summary>
		/// The number of words in the logical size of this BitSet.
		/// </summary>
		private int wordsInUse = 0;

		/// <summary>
		/// Whether the size of "words" is user-specified. If so, we assume
		/// the user knows what he's doing and try harder to preserve it.
		/// </summary>
		private bool sizeIsSticky = false;

		/// <summary>
		/// Creates a bit set whose initial size is large enough to explicitly
		/// represent bits with indices in the range {@code 0} through {@code nbits-1}. All bits are initially {@code false}.
		/// </summary>
		/// <param name="nbits"> the initial size of the bit set </param>
		/// <exception cref="NegativeArraySizeException"> if the specified initial size
		///         is negative </exception>
		public SimpleBitSet(int nbits)
		{
			// nbits can't be negative; size 0 is OK
			if (nbits < 0)
			{
				throw new System.IndexOutOfRangeException("nbits < 0: " + nbits);
			}

			initWords(nbits);
			sizeIsSticky = true;
		}

		private void initWords(int nbits)
		{
			words = new sbyte[nbits];
		}

		/// <summary>
		/// Creates a bit set using words as the internal representation.
		/// The last word (if there is one) must be non-zero.
		/// </summary>
		private SimpleBitSet(sbyte[] words)
		{
			this.words = words;
			this.wordsInUse = words.Length;
			checkInvariants();
		}

		/// <summary>
		/// Every public method must preserve these invariants.
		/// </summary>
		private void checkInvariants()
		{
			if ((wordsInUse == 0 || words[wordsInUse - 1] != UNSETTED_VAL) && (wordsInUse >= 0 && wordsInUse <= words.Length) && (wordsInUse == words.Length || words[wordsInUse] == UNSETTED_VAL))
			{
				return;
			}

			throw new System.IndexOutOfRangeException("checkInvariants");
		}

		/// <summary>
		/// Sets the field wordsInUse to the logical size in words of the bit set.
		/// WARNING:This method assumes that the number of words actually in use is
		/// less than or equal to the current value of wordsInUse!
		/// </summary>
		private void recalculateWordsInUse()
		{
			// Traverse the bitset until a used word is found
			int i;
			for (i = wordsInUse - 1; i >= 0; i--)
			{
				if (words[i] != UNSETTED_VAL)
				{
					break;
				}
			}

			wordsInUse = i + 1; // The new logical size
		}

		/// <summary>
		/// Returns a new bit set containing all the bits in the given byte array.
		/// </summary>
		public virtual SimpleBitSet valueOf(sbyte[] bytes)
		{
			int n;
			for (n = bytes.Length; n > 0 && bytes[n - 1] == UNSETTED_VAL; n--)
			{
				;
			}
			return new SimpleBitSet(arrayCopyOf(bytes, n));
		}

		private sbyte[] arrayCopyOf(sbyte[] preArray, int newSize)
		{
			sbyte[] _aftArray = new sbyte[newSize];
			int _loop = preArray.Length > newSize ? newSize : preArray.Length;
			for (int i = 0; i < _loop; i++)
			{
				_aftArray[i] = preArray[i];
			}

			return _aftArray;
		}

		/// <summary>
		/// Ensures that the BitSet can hold enough words.
		/// </summary>
		/// <param name="wordsRequired"> the minimum acceptable number of words. </param>
		private void ensureCapacity(int wordsRequired)
		{
			if (words.Length < wordsRequired)
			{
				// Allocate larger of doubled size or required size
				int request = Math.Max(2 * words.Length, wordsRequired);
				words = arrayCopyOf(words, request);
				sizeIsSticky = false;
			}
		}

		/// <summary>
		/// Ensures that the BitSet can accommodate a given wordIndex,
		/// temporarily violating the invariants. The caller must
		/// restore the invariants before returning to the user,
		/// possibly using recalculateWordsInUse().
		/// </summary>
		/// <param name="wordIndex"> the index to be accommodated. </param>
		private void expandTo(int wordIndex)
		{
			int wordsRequired = wordIndex + 1;
			if (wordsInUse < wordsRequired)
			{
				ensureCapacity(wordsRequired);
				wordsInUse = wordsRequired;
			}
		}

		/// <summary>
		/// Checks that fromIndex ... toIndex is a valid range of bit indices.
		/// </summary>
		private static void checkRange(int fromIndex, int toIndex)
		{
			if (fromIndex < 0)
			{
				throw new System.IndexOutOfRangeException("fromIndex < 0: " + fromIndex);
			}
			if (toIndex < 0)
			{
				throw new System.IndexOutOfRangeException("toIndex < 0: " + toIndex);
			}
			if (fromIndex > toIndex)
			{
				throw new System.IndexOutOfRangeException("fromIndex: " + fromIndex + " > toIndex: " + toIndex);
			}
		}

		private int getWordIndex(int bitIndex)
		{
			return bitIndex;
		}

		/// <summary>
		/// Sets the bit at the specified index to the complement of its
		/// current value.
		/// </summary>
		/// <param name="bitIndex"> the index of the bit to flip </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since 1.4 </exception>
		public virtual void flip(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new System.IndexOutOfRangeException("bitIndex < 0: " + bitIndex);
			}

			int wordIndex = getWordIndex(bitIndex);
			expandTo(wordIndex);

			sbyte _preVal = words[wordIndex];
			words[wordIndex] = (_preVal == UNSETTED_VAL ? SETTED_VAL : UNSETTED_VAL);

			recalculateWordsInUse();
			checkInvariants();
		}

		/// <summary>
		/// Sets each bit from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to the complement of its current
		/// value.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to flip </param>
		/// <param name="toIndex"> index after the last bit to flip </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since 1.4 </exception>
		public virtual void flip(int fromIndex, int toIndex)
		{
			checkRange(fromIndex, toIndex);

			if (fromIndex == toIndex)
			{
				return;
			}

			int startWordIndex = getWordIndex(fromIndex);
			int endWordIndex = getWordIndex(toIndex);
			expandTo(endWordIndex);

			for (int i = startWordIndex; i < endWordIndex; i++)
			{
				sbyte _preVal = words[i];
				words[i] = (_preVal == UNSETTED_VAL ? SETTED_VAL : UNSETTED_VAL);
			}

			recalculateWordsInUse();
			checkInvariants();
		}

		/// <summary>
		/// Sets the bit at the specified index to {@code true}.
		/// </summary>
		/// <param name="bitIndex"> a bit index </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since JDK1.0 </exception>
		public virtual void set(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new System.IndexOutOfRangeException("bitIndex < 0: " + bitIndex);
			}

			int wordIndex = getWordIndex(bitIndex);
			expandTo(wordIndex);

			words[wordIndex] = SETTED_VAL; // Restores invariants

			checkInvariants();
		}

		/// <summary>
		/// Sets the bit at the specified index to the specified value.
		/// </summary>
		/// <param name="bitIndex"> a bit index </param>
		/// <param name="value"> a boolean value to set </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since 1.4 </exception>
		public virtual void set(int bitIndex, bool value)
		{
			if (value)
			{
				set(bitIndex);
			}
			else
			{
				clear(bitIndex);
			}
		}

		/// <summary>
		/// Sets the bits from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to {@code true}.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to be set </param>
		/// <param name="toIndex"> index after the last bit to be set </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since 1.4 </exception>
		public virtual void set(int fromIndex, int toIndex)
		{
			checkRange(fromIndex, toIndex);

			if (fromIndex == toIndex)
			{
				return;
			}

			// Increase capacity if necessary
			int startWordIndex = getWordIndex(fromIndex);
			int endWordIndex = getWordIndex(toIndex - 1);
			expandTo(endWordIndex);

			for (int i = startWordIndex; i <= endWordIndex; i++)
			{
				words[i] = SETTED_VAL;
			}

			checkInvariants();
		}

		/// <summary>
		/// Sets the bits from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to the specified value.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to be set </param>
		/// <param name="toIndex"> index after the last bit to be set </param>
		/// <param name="value"> value to set the selected bits to </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since 1.4 </exception>
		public virtual void set(int fromIndex, int toIndex, bool value)
		{
			if (value)
			{
				set(fromIndex, toIndex);
			}
			else
			{
				clear(fromIndex, toIndex);
			}
		}

		/// <summary>
		/// Sets the bit specified by the index to {@code false}.
		/// </summary>
		/// <param name="bitIndex"> the index of the bit to be cleared </param>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since JDK1.0 </exception>
		public virtual void clear(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new System.IndexOutOfRangeException("bitIndex < 0: " + bitIndex);
			}

			int wordIndex = getWordIndex(bitIndex);
			if (wordIndex >= wordsInUse)
			{
				return;
			}

			words[wordIndex] = UNSETTED_VAL;

			recalculateWordsInUse();
			checkInvariants();
		}

		/// <summary>
		/// Sets the bits from the specified {@code fromIndex} (inclusive) to the
		/// specified {@code toIndex} (exclusive) to {@code false}.
		/// </summary>
		/// <param name="fromIndex"> index of the first bit to be cleared </param>
		/// <param name="toIndex"> index after the last bit to be cleared </param>
		/// <exception cref="IndexOutOfBoundsException"> if {@code fromIndex} is negative,
		///         or {@code toIndex} is negative, or {@code fromIndex} is
		///         larger than {@code toIndex}
		/// @since 1.4 </exception>
		public virtual void clear(int fromIndex, int toIndex)
		{
			checkRange(fromIndex, toIndex);

			if (fromIndex == toIndex)
			{
				return;
			}

			int startWordIndex = getWordIndex(fromIndex);
			if (startWordIndex >= wordsInUse)
			{
				return;
			}

			int endWordIndex = getWordIndex(toIndex);
			if (endWordIndex > wordsInUse)
			{
				endWordIndex = wordsInUse;
			}

			for (int i = startWordIndex; i < endWordIndex; i++)
			{
				words[i] = UNSETTED_VAL;
			}

			recalculateWordsInUse();
			checkInvariants();
		}

		/// <summary>
		/// Sets all of the bits in this BitSet to {@code false}.
		/// 
		/// @since 1.4
		/// </summary>
		public virtual void clear()
		{
			while (wordsInUse > 0)
			{
				words[--wordsInUse] = UNSETTED_VAL;
			}
		}

		/// <summary>
		/// Returns the value of the bit with the specified index. The value
		/// is {@code true} if the bit with the index {@code bitIndex} is currently set in this {@code BitSet}; otherwise, the result
		/// is {@code false}.
		/// </summary>
		/// <param name="bitIndex"> the bit index </param>
		/// <returns> the value of the bit with the specified index </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative </exception>
		public virtual bool get(int bitIndex)
		{
			if (bitIndex < 0)
			{
				throw new System.IndexOutOfRangeException("bitIndex < 0: " + bitIndex);
			}

			checkInvariants();

			int wordIndex = getWordIndex(bitIndex);
			return (wordIndex < wordsInUse) && (words[wordIndex] != UNSETTED_VAL);
		}

		/// <summary>
		/// Returns the index of the first bit that is set to {@code true} that occurs on or after the specified starting index. If no such
		/// bit exists then {@code -1} is returned.
		/// 
		/// <para>
		/// To iterate over the {@code true} bits in a {@code BitSet}, use the following loop:
		/// 
		/// <pre>
		/// {@code
		/// for (int i = bs.nextSetBit(0); i >= 0; i = bs.nextSetBit(i+1)) {
		///     // operate on index i here
		/// }}
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the next set bit, or {@code -1} if there
		///         is no such bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since 1.4 </exception>
		public virtual int nextSetBit(int fromIndex)
		{
			if (fromIndex < 0)
			{
				throw new System.IndexOutOfRangeException("fromIndex < 0: " + fromIndex);
			}

			checkInvariants();

			int u = getWordIndex(fromIndex);
			if (u >= wordsInUse)
			{
				return -1;
			}

			sbyte word = words[u];

			while (true)
			{
				if (word != UNSETTED_VAL)
				{
					return u;
				}
				if (++u == wordsInUse)
				{
					return -1;
				}
				word = words[u];
			}
		}

		/// <summary>
		/// Returns the index of the first bit that is set to {@code false} that occurs on or after the specified starting index.
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the next clear bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is negative
		/// @since 1.4 </exception>
		public virtual int nextClearBit(int fromIndex)
		{
			// Neither spec nor implementation handle bitsets of maximal length.
			// See 4816253.
			if (fromIndex < 0)
			{
				throw new System.IndexOutOfRangeException("fromIndex < 0: " + fromIndex);
			}

			checkInvariants();

			int u = getWordIndex(fromIndex);
			if (u >= wordsInUse)
			{
				return fromIndex;
			}

			sbyte word = words[u];

			while (true)
			{
				if (word == UNSETTED_VAL)
				{
					return u;
				}
				if (++u == wordsInUse)
				{
					return wordsInUse;
				}
				word = words[u];
			}
		}

		/// <summary>
		/// Returns the index of the nearest bit that is set to {@code true} that occurs on or before the specified starting index.
		/// If no such bit exists, or if {@code -1} is given as the
		/// starting index, then {@code -1} is returned.
		/// 
		/// <para>
		/// To iterate over the {@code true} bits in a {@code BitSet}, use the following loop:
		/// 
		/// <pre>
		/// {@code
		/// for (int i = bs.length(); (i = bs.previousSetBit(i-1)) >= 0; ) {
		///     // operate on index i here
		/// }}
		/// </pre>
		/// 
		/// </para>
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the previous set bit, or {@code -1} if there
		///         is no such bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is less
		///         than {@code -1}
		/// @since 1.7 </exception>
		public virtual int previousSetBit(int fromIndex)
		{
			if (fromIndex < 0)
			{
				if (fromIndex == -1)
				{
					return -1;
				}
				throw new System.IndexOutOfRangeException("fromIndex < -1: " + fromIndex);
			}

			checkInvariants();

			int u = getWordIndex(fromIndex);
			if (u >= wordsInUse)
			{
				return length() - 1;
			}

			sbyte word = words[u];

			while (true)
			{
				if (word != UNSETTED_VAL)
				{
					return u;
				}
				if (u-- == 0)
				{
					return -1;
				}
				word = words[u];
			}
		}

		/// <summary>
		/// Returns the index of the nearest bit that is set to {@code false} that occurs on or before the specified starting index.
		/// If no such bit exists, or if {@code -1} is given as the
		/// starting index, then {@code -1} is returned.
		/// </summary>
		/// <param name="fromIndex"> the index to start checking from (inclusive) </param>
		/// <returns> the index of the previous clear bit, or {@code -1} if there
		///         is no such bit </returns>
		/// <exception cref="IndexOutOfBoundsException"> if the specified index is less
		///         than {@code -1}
		/// @since 1.7 </exception>
		public virtual int previousClearBit(int fromIndex)
		{
			if (fromIndex < 0)
			{
				if (fromIndex == -1)
				{
					return -1;
				}
				throw new System.IndexOutOfRangeException("fromIndex < -1: " + fromIndex);
			}

			checkInvariants();

			int u = getWordIndex(fromIndex);
			if (u >= wordsInUse)
			{
				return fromIndex;
			}

			sbyte word = words[u];

			while (true)
			{
				if (word == UNSETTED_VAL)
				{
					return u;
				}
				if (u-- == 0)
				{
					return -1;
				}
				word = words[u];
			}
		}

		public virtual int length()
		{
			return wordsInUse;
		}

		/// <summary>
		/// Returns true if this {@code BitSet} contains no bits that are set
		/// to {@code true}.
		/// </summary>
		/// <returns> boolean indicating whether this {@code BitSet} is empty
		/// @since 1.4 </returns>
		public virtual bool Empty
		{
			get
			{
				return wordsInUse == 0;
			}
		}

		/// <summary>
		/// Returns true if the specified {@code BitSet} has any bits set to {@code true} that are also set to {@code true} in this {@code BitSet}.
		/// </summary>
		/// <param name="set"> {@code BitSet} to intersect with </param>
		/// <returns> boolean indicating whether this {@code BitSet} intersects
		///         the specified {@code BitSet}
		/// @since 1.4 </returns>
		public virtual bool intersects(SimpleBitSet set)
		{
			for (int i = Math.Min(wordsInUse, set.wordsInUse) - 1; i >= 0; i--)
			{
				if ((words[i] & set.words[i]) != 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the number of bits set to {@code true} in this {@code BitSet}.
		/// </summary>
		/// <returns> the number of bits set to {@code true} in this {@code BitSet}
		/// @since 1.4 </returns>
		public virtual int cardinality()
		{
			int sum = 0;
			for (int i = 0; i < wordsInUse; i++)
			{
				if (words[i] == SETTED_VAL)
				{
					sum++;
				}
			}

			return sum;
		}

		/// <summary>
		/// Performs a logical <b>AND</b> of this target bit set with the
		/// argument bit set. This bit set is modified so that each bit in it
		/// has the value {@code true} if and only if it both initially
		/// had the value {@code true} and the corresponding bit in the
		/// bit set argument also had the value {@code true}.
		/// </summary>
		/// <param name="set"> a bit set </param>
		public virtual void and(SimpleBitSet set)
		{
			if (this == set)
			{
				return;
			}

			while (wordsInUse > set.wordsInUse)
			{
				words[--wordsInUse] = UNSETTED_VAL;
			}

			// Perform logical AND on words in common
			for (int i = 0; i < wordsInUse; i++)
			{
				words[i] &= set.words[i];
			}

			recalculateWordsInUse();
			checkInvariants();
		}

		/// <summary>
		/// Performs a logical <b>OR</b> of this bit set with the bit set
		/// argument. This bit set is modified so that a bit in it has the
		/// value {@code true} if and only if it either already had the
		/// value {@code true} or the corresponding bit in the bit set
		/// argument has the value {@code true}.
		/// </summary>
		/// <param name="set"> a bit set </param>
		public virtual void or(SimpleBitSet set)
		{
			if (this == set)
			{
				return;
			}

			int wordsInCommon = Math.Min(wordsInUse, set.wordsInUse);

			if (wordsInUse < set.wordsInUse)
			{
				ensureCapacity(set.wordsInUse);
				wordsInUse = set.wordsInUse;
			}

			// Perform logical OR on words in common
			for (int i = 0; i < wordsInCommon; i++)
			{
				words[i] |= set.words[i];
			}

			// Copy any remaining words
			if (wordsInCommon < set.wordsInUse)
			{
				arraycopy(set.words, wordsInCommon, words, wordsInCommon, wordsInUse - wordsInCommon);
			}

			// recalculateWordsInUse() is unnecessary
			checkInvariants();
		}

		private void arraycopy(sbyte[] srcArry, int srcPos, sbyte[] destArry, int destPos, int length)
		{
			for (int i = 0; i < length; i++)
			{
				destArry[destPos + i] = srcArry[srcPos + i];
			}
		}

		/// <summary>
		/// Performs a logical <b>XOR</b> of this bit set with the bit set
		/// argument. This bit set is modified so that a bit in it has the
		/// value {@code true} if and only if one of the following
		/// statements holds:
		/// <ul>
		/// <li>The bit initially has the value {@code true}, and the corresponding bit in the argument has the value {@code false}.
		/// <li>The bit initially has the value {@code false}, and the corresponding bit in the argument has the value {@code true}.
		/// </ul>
		/// </summary>
		/// <param name="set"> a bit set </param>
		public virtual void xor(SimpleBitSet set)
		{
			int wordsInCommon = Math.Min(wordsInUse, set.wordsInUse);

			if (wordsInUse < set.wordsInUse)
			{
				ensureCapacity(set.wordsInUse);
				wordsInUse = set.wordsInUse;
			}

			// Perform logical XOR on words in common
			for (int i = 0; i < wordsInCommon; i++)
			{
				if (words[i] != set.words[i])
				{
					words[i] = SETTED_VAL;
				}
				else
				{
					words[i] = UNSETTED_VAL;
				}
			}

			// Copy any remaining words
			if (wordsInCommon < set.wordsInUse)
			{
				this.arraycopy(set.words, wordsInCommon, words, wordsInCommon, set.wordsInUse - wordsInCommon);
			}

			recalculateWordsInUse();
			checkInvariants();
		}

		/// <summary>
		/// Returns the hash code value for this bit set. The hash code depends
		/// only on which bits are set within this {@code BitSet}.
		/// 
		/// <para>
		/// The hash code is defined to be the result of the following calculation:
		/// 
		/// <pre>
		/// {@code
		/// public int hashCode() {
		///     long h = 1234;
		///     long[] words = toLongArray();
		///     for (int i = words.length; --i >= 0; )
		///         h ^= words[i] * (i + 1);
		///     return (int)((h >> 32) ^ h);
		/// }}
		/// </pre>
		/// 
		/// Note that the hash code changes if the set of bits is altered.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the hash code value for this bit set </returns>
		public override int GetHashCode()
		{
			long h = 1234;
			for (int i = wordsInUse; --i >= 0;)
			{
				h ^= words[i] * (i + 1);
			}

			return (int)((h >> 32) ^ h);
		}

		/// <summary>
		/// Returns the number of bits of space actually in use by this {@code BitSet} to represent bit values.
		/// The maximum element in the set is the size - 1st element.
		/// </summary>
		/// <returns> the number of bits currently in this bit set </returns>
		public virtual int size()
		{
			return words.Length;
		}

		/// <summary>
		/// Compares this object against the specified object.
		/// The result is {@code true} if and only if the argument is
		/// not {@code null} and is a {@code Bitset} object that has
		/// exactly the same set of bits set to {@code true} as this bit
		/// set. That is, for every nonnegative {@code int} index {@code k},
		/// 
		/// <pre>
		/// ((BitSet) obj).get(k) == this.get(k)
		/// </pre>
		/// 
		/// must be true. The current sizes of the two bit sets are not compared.
		/// </summary>
		/// <param name="obj"> the object to compare with </param>
		/// <returns> {@code true} if the objects are the same; {@code false} otherwise </returns>
		/// <seealso cref= #size() </seealso>
		public override bool Equals(object obj)
		{
			if (!(obj is SimpleBitSet))
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}

			SimpleBitSet set = (SimpleBitSet) obj;

			checkInvariants();
			set.checkInvariants();

			if (wordsInUse != set.wordsInUse)
			{
				return false;
			}

			// Check words in use by both BitSets
			for (int i = 0; i < wordsInUse; i++)
			{
				if (words[i] != set.words[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Cloning this {@code BitSet} produces a new {@code BitSet} that is equal to it.
		/// The clone of the bit set is another bit set that has exactly the
		/// same bits set to {@code true} as this bit set.
		/// </summary>
		/// <returns> a clone of this bit set </returns>
		/// <seealso cref= #size() </seealso>
		public virtual object clone()
		{
			if (!sizeIsSticky)
			{
				trimToSize();
			}

			SimpleBitSet result = valueOf(this.words);
			return result;
		}

		/// <summary>
		/// Attempts to reduce internal storage used for the bits in this bit set.
		/// Calling this method may, but is not required to, affect the value
		/// returned by a subsequent call to the <seealso cref="#size()"/> method.
		/// </summary>
		private void trimToSize()
		{
			if (wordsInUse != words.Length)
			{
				words = this.arrayCopyOf(words, wordsInUse);
				checkInvariants();
			}
		}

		/// <summary>
		/// Returns a string representation of this bit set. For every index
		/// for which this {@code BitSet} contains a bit in the set
		/// state, the decimal representation of that index is included in
		/// the result. Such indices are listed in order from lowest to
		/// highest, separated by ",&nbsp;" (a comma and a space) and
		/// surrounded by braces, resulting in the usual mathematical
		/// notation for a set of integers.
		/// 
		/// <para>
		/// Example:
		/// 
		/// <pre>
		/// BitSet drPepper = new BitSet();
		/// </pre>
		/// 
		/// Now {@code drPepper.toString()} returns "{@code ".
		/// </para>
		/// <para>
		/// 
		/// <pre>
		/// drPepper.set(2);
		/// </pre>Now {@code drPepper.toString()} returns "{@code 2} ".
		/// </para>
		/// <para>
		/// 
		/// <pre>
		/// drPepper.set(4);
		/// drPepper.set(10);
		/// </pre>
		/// 
		/// Now {@code drPepper.toString()} returns "{@code 2, 4, 10} ".
		/// 
		/// </para>
		/// </summary>
		/// <returns> a string representation of this bit set </returns>
		public override string ToString()
		{
			checkInvariants();

			int numBits = (wordsInUse > 128) ? cardinality() : wordsInUse;
			StringBuilder b = new StringBuilder(6 * numBits + 2);
			b.Append('{');

			int i = nextSetBit(0);
			if (i != -1)
			{
				b.Append(i);
				for (i = nextSetBit(i + 1); i >= 0; i = nextSetBit(i + 1))
				{
					int endOfRun = nextClearBit(i);
					do
					{
						b.Append(", ").Append(i);
					} while (++i < endOfRun);
				}
			}

			b.Append('}');
			return b.ToString();
		}
	}

}