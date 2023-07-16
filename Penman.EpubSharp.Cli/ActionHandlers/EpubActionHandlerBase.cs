﻿using System.IO.Abstractions;

namespace Penman.EpubSharp.Cli.ActionHandlers;

public class EpubActionHandlerBase
{
    protected readonly IFileSystem FileSystem;
    protected EpubBook? EpubToProcess;
    protected EpubWriter ? EpubWriter;

    public EpubActionHandlerBase(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    protected bool RetrieveAndValidateEpubSuccessful(EpubManipulatorOptionsBase options)
    {
        if (!FileSystem.File.Exists(options.InputEpub))
        {
            Console.WriteLine($"Cannot find file input-epub {options.InputEpub}");
            return false;
        }

        var epubData = FileSystem.File.ReadAllBytes(options.InputEpub);
        EpubToProcess = EpubReader.Read(epubData);
        EpubWriter = new EpubWriter(EpubToProcess);

        return true;
    }

    protected byte[] FetchByteArrayForFile(string pathToFile)
    {
        return FileSystem.File.ReadAllBytes(pathToFile);
    }

    protected bool TryParseImageFormat(string pathToImage, out ImageFormat? imageFormat)
    {
        var fileInfo = FileSystem.FileInfo.New(pathToImage);

        switch (fileInfo.Extension.Replace(".", ""))
        {
            case "jpg":
            case "jpeg":
                imageFormat = ImageFormat.Jpeg;
                return true;
            case "png":
                imageFormat = ImageFormat.Png;
                return true;
            case "gif":
                imageFormat = ImageFormat.Gif;
                return true;
            case "svg":
                imageFormat = ImageFormat.Svg;
                return true;
            default:
                imageFormat = null;
                return false;
        }
    }

    protected void SaveChanges(EpubManipulatorOptionsBase options)
    {
        var finalOutputPath = string.IsNullOrEmpty(options.OutputEpub) ? options.InputEpub : options.OutputEpub;

        EpubWriter?.Write(finalOutputPath);
    }
}