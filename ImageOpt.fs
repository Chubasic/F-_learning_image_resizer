module ImageOpt


// For more information see https://aka.ms/fsharp-console-apps
// Created by Orest

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


// let readLn = Console.ReadLine()
// printfn $"{readLn}"


type LoadedImage = {
    FileInfo: FileInfo
    Image: SKImage
}

let inputDir=
    let inputPath = defaultFilesLocatios.DefaulyConf.Locations.INPUT_PATH
    let dirInfo: DirectoryInfo = DirectoryInfo(inputPath)
    if not dirInfo.Exists then  
        printfn ".:Input directory does not exist:.\n Created\n\b" 
        dirInfo.Create()
        
    dirInfo 
        

let images (dirInfo: DirectoryInfo): List<FileInfo> = List.ofArray (dirInfo.GetFiles())
    
let loadImage (fileInfo: FileInfo): LoadedImage =
    let inputPath = defaultFilesLocatios.DefaulyConf.Locations.INPUT_PATH
    let image = SKImage.FromEncodedData($"{inputPath}/{fileInfo.Name}")
    printfn $"Loading file {inputPath}/{fileInfo.Name}"
    {FileInfo = fileInfo; Image=image}
    
    

let webOptimized (bitmap: SKBitmap): SKData = 
    bitmap.Encode(SKEncodedImageFormat.Jpeg, 60)
    


let save (fileInfo: FileInfo) (data: SKData) = 
    let outputPath = defaultFilesLocatios.DefaulyConf.Locations.OUTPUT_PATH
    use stream = new  FileStream($"{outputPath}/{fileInfo.Name}", FileMode.Create)
    do stream.Write(data.ToArray(), 0, int data.Size)


(* Actions:
    - TODO: Resize to demensions.
    - TODO: Web Optimization.
    - TODO: Make B&W
*)

let helpMsg = printfn "
::Simple transformation util::\n
--bw --> Make image black&white\n
--res --> Resize image to dimensions\n
--web --> Optimize web image to use in web\n"


let parsePattern input = 
    let pattern = @"--res\s+(\d+)\s+(\d+)\s+--bw\s+--web"
    let regex = Regex(pattern)
    match regex.Match(input) with
    (*
    Regex Match: The regex.Match(input) function is used to find a match in the input string.
    Match Groups: If a match is found, m.Groups.[1].Value and m.Groups.[2].Value extract 
    the captured width and height as strings, which are then converted to integers.
    *)
    | m when m.Success ->
        let width = int m.Groups.[1].Value
        let height = int m.Groups.[2].Value
        Some (width, height)
    | _ -> None



let parseAction args = List.map (fun arg -> 
    match arg with
    | "--bw" -> printfn "Black and White"
    | "--res" -> printfn "Resize"
    | "--web" -> printfn "Web opt"
    | _ -> helpMsg
    )

let result = inputDir |> images |> List.map (fun file -> (loadImage file))
printf $"Files names: %A{result}"
 
def main():
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
// 	main()

