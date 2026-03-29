# DQD.ForkPTY

DQD.ForkPTY is a .NET library that bundles native binary components under the hood to enable the use of `forkpty()` across multiple supported platforms. Using `forkpty` (or `fork` in general) is not possible from managed code; a wrapper written in a bare metal language is needed.

## Usage

```
  public static class Forker
  {
    public static ForkResult ForkPTYAndExec(PTYConfiguration configuration, string fileName, string?[]? argv = null);
  }

  public class PTYConfiguration
  {
    public Size CharacterSize;
    public Size PixelSize;
  }

  public struct Size
  {
    public int Width;
    public int Height;
  }
```

Set up a suitable `PTYConfiguration` instance and call `Forker.ForkPTYAndExec` with your designed program filename and (optional) arguments.

* If the platform is not supported, a `PlatformNotSupportedException` will be thrown.
* If an error occurs, a `Win32Exception` will be thrown with the captured `errno` value.
* If the call succeeds, a `ForkResult` will be returned that contains the child process ID and a `Stream` wrapping its PTY.
    * `ForkResult` implements `IDisposable` and disposes the PTY stream when it is disposed.

## Example

See the `DQD.ForkPTY.Playground` project for a simple example of use.

## Contributing

Contributions -- issues, pull requests, discussion -- are welcome on the [GitHub page](https://github.com/logiclrd/DQD.ForkPTY/).

## License

DQD.ForkPTY is provided under the MIT open source license. See [`LICENSE.md`](LICENSE.md) for more details.
