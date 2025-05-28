# Help

Dev Numbers is an extension for Command Palette. It converts numbers between different formats, including decimal, hexadecimal, binary, octal, and character formats. It supports various styles such as C, C#, C++, LISP R-style, VB-style, and more.

---------------------
# Number Formats

The extension automatically detects the number format based on the input and converts it to the specified output format. You can also override the format by specifying the desired format in the settings.

- C style
    - `0xFFFF` for hexadecimal format.
    - `0b1010` for binary format.
    - `123` for decimal format.
    - `0567` for octal format.
- C# style - C style with optional underscores `_` as a digit separator.
    - `0xFF_FF` for hexadecimal format.
    - `0b1010_1010` for binary format.
- C++ style - C style with optional apostrophes `'` as a digit separator.
    - `0x'FF'FF` for hexadecimal format.
    - `0b'1010'1010` for binary format.
- LISP R-style format
    - `10r123` for decimal format.
    - `2r1010` for binary format.
    - `16rFF` for hexadecimal format.
- Suffix format
    - `FFh` or `FFhex` for hexadecimal format.
    - `777o` or `777oct` for octal format.
    - `1010bin` for binary format.
    - `123dec` for decimal format.
- Prefix format
    - `hFF` for hexadecimal format.
    - `o777` for octal format.
- Symbol format:
    - `#FF` or `$FF` for hexadecimal format.
    - `@777` for octal format.
    - `%1010` for binary format.
- VB-style format
    - `&HFF` for hexadecimal format.
    - `&B1010` for binary format.
    - `&O777` for octal format.
    - `&D123` for decimal format.
    - Underscore `_` is supported as digit group separators.
- ADA -style format
    - `16#FFFF#` for hexadecimal format.
    - `2#1010#` for binary format.
    - `8#777#` for octal format.
    - `10#123#` for decimal format.
- character format - a character or symbol is converted to its ASCII code.
    - `'a'` or `'@'` or `'💚'`
    - `'\x0041'` for hexadecimal character code.
    - `'\u0041'` for Unicode code point (4 hex digits).
    - `'\U0001F49A'` for Unicode character code (8 hex digits).
    - `'U+1F49A'` for Unicode character code (hexadecimal format).
    - `'U-1F49A'` legacy format.


---------------------
# Parameters
## Bit Length `/length:<value>`

Specifies the bit length of the number to be converted. The value must a positive integer or a valid keyword (see below).

Extension automatically detects the bit length based on the input number, but you can override it by specifying the `/length` parameter.

If the requested length is less than the actual length of the number, the number will be truncated to fit the specified length and warning will be shown in the output.



Usage:

```
123 /length:BYTE
```
```
0xFFFF /length:12
```



The following keywords are supported:

| Value   |  Length |
| ------- | ------: |
| `BYTE`  |  8 bits |
| `WORD`  | 16 bits |
| `DWORD` | 32 bits |
| `QWORD` | 64 bits |
| `SHORT` | 16 bits |
| `INT`   | 32 bits |
| `LONG`  | 64 bits |
| `INT8`  |  8 bits |
| `INT16` | 16 bits |
| `INT32` | 32 bits |
| `INT64` | 64 bits |

---------------------
# Examples

Convert C# hexadecimal number

```
0x874050
```

Convert a decimal number with bit length

```
-123 ... 85h
-123 /length:DWORD ... FFFFFF85h
```

