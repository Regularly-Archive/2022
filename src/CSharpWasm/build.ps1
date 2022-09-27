mcs /target:library -out:./CSharpWasm.dll /noconfig /nostdlib /r:d:/mono-wasm/wasm-bcl/wasm/mscorlib.dll /r:d:/mono-wasm/wasm-bcl/wasm/System.dll /r:d:/mono-wasm/wasm-bcl/wasm/System.Core.dll /r:d:/mono-wasm/wasm-bcl/wasm/Facades/netstandard.dll /r:d:/mono-wasm/wasm-bcl/wasm/System.Net.Http.dll /r:d:/mono-wasm/framework/WebAssembly.Bindings.dll /r:d:/mono-wasm/framework/WebAssembly.Net.Http.dll Program.cs
mono "d:/mono-wasm/packager.exe" --copy=always --out=./publish CSharpWasm.dll
rm ./CSharpWasm.dll
cp ./index.html ./publish/index.html
cd ./publish
python -m http.server
