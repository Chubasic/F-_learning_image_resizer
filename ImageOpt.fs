module ImageOpt
// For more information see https://aka.ms/fsharp-console-apps

// from PIL import Image
// import os, os.path
// import sys
// import configparser


open System
open SkiaSharp
open System.IO
open System.Text.RegularExpressions
open FSharp.Configuration


type FilesLocationConfig = YamlConfig<"Conf.yaml">
let defaultFilesLocatios = FilesLocationConfig()


printfn "Hello from F#"
printfn $"{defaultFilesLocatios.DefaulyConf.Locations.INPUT_PATH}"
printfn $"{defaultFilesLocatios.DefaulyConf.Locations.OUTPUT_PATH}"


// Use startegy pattern


type LoadedImage = { FileInfo: FileInfo; Image: SKImage }

type ImageBitmap =
    { FileInfo: FileInfo
      imageBitmap: SKBitmap }

type ImageEncoded =
    { FileInfo: FileInfo
      ImageEncoded: SKData }

type ActionsConfig =
    { Height: Option<int32>
      Width: Option<int32>
      WebOpt: Boolean
      BlackAndWhite: Boolean } // Temporary


let inputDir =
    let inputPath = defaultFilesLocatios.DefaulyConf.Locations.INPUT_PATH
    let dirInfo: DirectoryInfo = DirectoryInfo(inputPath)

    if not dirInfo.Exists then
        printfn ".:Input directory does not exist:.\n Created\n\b"
        dirInfo.Create()

    dirInfo


let images (dirInfo: DirectoryInfo) : List<FileInfo> = List.ofArray (dirInfo.GetFiles())


let loadImage (fileInfo: FileInfo) : LoadedImage =
    let inputPath = defaultFilesLocatios.DefaulyConf.Locations.INPUT_PATH
    let image = SKImage.FromEncodedData($"{inputPath}/{fileInfo.Name}")
    printfn $"Loading file {inputPath}/{fileInfo.Name}"
    { FileInfo = fileInfo; Image = image }

let helpMsg =
    printfn
        "
::Simple transformation util::\n
--bw --> Make image black&white\n
--res --> Resize image to dimensions\n
--web --> Optimize web image to use in web\n"

let parseWebOptArg (input: string) (parsed: ActionsConfig) : ActionsConfig =
    let pattern = @"(--web)"

    match Regex.IsMatch(input, pattern) with
    | true ->
        { WebOpt = true
          Width = parsed.Height
          Height = parsed.Height
          BlackAndWhite = parsed.BlackAndWhite }
    | _ -> parsed


let parseBlackAndWhiteArg (input: string) (parsed: ActionsConfig) : ActionsConfig =
    let pattern = @"(--bw)"

    match Regex.IsMatch(input, pattern) with
    | true ->
        { BlackAndWhite = true
          Width = parsed.Height
          Height = parsed.Height
          WebOpt = parsed.WebOpt }
    | _ -> parsed


let parseResizeArg (input: string) (parsed: ActionsConfig) : ActionsConfig =
    let pattern = @"--res\s+(\d+)\s+(\d+)"
    let regex = Regex(pattern)

    match regex.Match(input) with
    | m when m.Success ->
        let width = int m.Groups.[1].Value
        let height = int m.Groups.[2].Value
        let restArgs = regex.Replace(input, "").Trim().Split(" ")

        { Width = Some width
          Height = Some height
          BlackAndWhite = parsed.BlackAndWhite
          WebOpt = parsed.WebOpt }

    | _ -> parsed


let parseArgs input : ActionsConfig =
    let withCases =
        parseBlackAndWhiteArg input >> parseResizeArg input >> parseWebOptArg input

    withCases
        { Width = None
          Height = None
          WebOpt = false
          BlackAndWhite = false }


// No need to bitmap for web opt
// Produces black square images

let toBitmap (loadedImage: LoadedImage) : ImageBitmap =
    { FileInfo = loadedImage.FileInfo
      imageBitmap = new SKBitmap(loadedImage.Image.Info) }

(* Actions:
    - TODO: Resize to demensions.
    - TODO: Web Optimization.
    - TODO: Make B&W
*)


let webOptimized (loadedImage: LoadedImage) : ImageEncoded =
    { FileInfo = loadedImage.FileInfo
      ImageEncoded = loadedImage.Image.Encode(SKEncodedImageFormat.Jpeg, 70) }

let defaultEncode (loadedImage: LoadedImage) : ImageEncoded =
    { FileInfo = loadedImage.FileInfo
      ImageEncoded = loadedImage.Image.Encode(SKEncodedImageFormat.Jpeg, 100) }

let blackAndWhite (image) = failwith "Not implemented"

let resize image = failwith "Not implemented"



let saveFiles (encodedImages: List<ImageEncoded>) =
    let save (encodedImage: ImageEncoded) =
        let fileName = encodedImage.FileInfo.Name
        let data = encodedImage.ImageEncoded.ToArray()
        let fileSize = int encodedImage.ImageEncoded.Size

        let outputPath = defaultFilesLocatios.DefaulyConf.Locations.OUTPUT_PATH
        use stream = new FileStream($"{outputPath}/{fileName}", FileMode.Create)
        do stream.Write(data, 0, fileSize)

    List.map save encodedImages

let processImage (config: ActionsConfig) (loadedImage: LoadedImage) =
    loadedImage
    // |> toBitmap
    |> if config.WebOpt then webOptimized else defaultEncode

let processImages (configuredProcessor: LoadedImage -> ImageEncoded) (images: List<LoadedImage>) =
    List.map configuredProcessor images


let main =
    let readLn = Console.ReadLine()
    printfn $"Input - {readLn} \n"
    let config = readLn |> parseArgs

    printfn $"Parsed config {config} \n"

    let configuredProcessor = processImage config

    let loadedImages =
        inputDir
        |> images
        |> List.map (fun file -> loadImage file)
        |> processImages configuredProcessor
        |> saveFiles

    printf $"Loaded files: %A{loadedImages} \n"






// 	"""
// 	CONFIGURATIONS
// 	"""
// 	config = configparser.ConfigParser()
// 	config.read("configurations.cfg")
// 	input_path = config.get('CONFIGS', 'input_path')
// 	output_path = config.get('CONFIGS', 'output_path')
// 	water_path = config.get('CONFIGS', 'water_path')
// 	Width = int(config.get('CONFIGS', 'width'))
// 	Height = int(config.get('CONFIGS', 'height'))

// 	os.chdir(output_path)
// 	for wfiles in os.listdir(output_path):
// 		watermark(wfiles, output_path, Width, Height)


// 	os.chdir(input_path)
// 	for files in os.listdir(input_path):
// 		resize(files, Width, Height, output_path)


// """
// Open file(s)and place watermark on image
// """
// def resize(files, Width, Height, output_path):

// 	photo = Image.open(files).convert('RGBA')
// 	photoW, photoH = photo.size
// 	if Width is None and Height is not None:
// 		photoW = (photoW * Height) / photoH
// 		photoH = Height
// 	elif Width is not None and Height is None:
// 		photoH = (photoH * Width) / photoW
// 		photoW = Width
// 	elif Width is not None and Height is not None:
// 		photoW = Width
// 		photoH = Width
// 	layer = Image.new('RGBA', (Width, Height), (0,0,0,0))
// 	photo_resize = photo.resize((int(photoW), int(photoH)), Image.LANCZOS)
// 	insert = ((Width - photoW), (Height - photoH))
// 	layer.paste(photo_resize, insert)
// 	layer.save(os.path.join(output_path, files))


// def watermark(wfiles, output_path, Width, Height):
// 	watermark = Image.open(wfiles)
// 	if watermark.mode != 'RGBA':
// 		watermark = watermark.convert('RGBA')
// 	photo = Image.open(wfiles)
// 	waterW, waterH = watermark.size
// 	if Width is None and Height is not None:
// 		waterW = (waterW * Height) / waterH
// 		waterH = Height
// 	elif Width is not None and Height is None:
// 		waterH = (waterH * Width) / waterW
// 		waterW = Width
// 	elif Width is not None and Height is not None:
// 		waterW = Width
// 		waterH = Width
// 	watermark = watermark.resize((int(waterW), int(waterH)), Image.LANCZOS)
// 	#layer = Image.new('RGBA', (Width, Height), (300, 300, 100, 100))
// 	insert_water = ((Width - waterW), (Height - waterH))
// 	photo.paste(watermark, insert_water)
// 	Image.composite_alpha(photo, watermark).save(os.path.join(output_path, wfiles))


// if __name__ == '__main__':
// main
