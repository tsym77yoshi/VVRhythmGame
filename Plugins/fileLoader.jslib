mergeInto(LibraryManager.library, {
  FileLoad: function(){
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = '.vvproj';
    input.onchange = event => {
      const reader = new FileReader();
      reader.readAsText(event.target.files[0]);
      reader.addEventListener( 'load', function() {
        console.log(reader.result);
        SendMessage('LoadButton', 'onFileLoaded', reader.result);
      })
    };
    input.click();
  },
  MusicFileLoad: function(){
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'audio/wav';
    input.onchange = event => {
      const reader = new FileReader();
      reader.readAsArrayBuffer(event.target.files[0]);
      reader.addEventListener( 'load', function() {
        console.log(reader.result);
        SendMessage('LoadButton', 'onMusicFileLoaded', reader.result);
      })
    };
    input.click();
  },
  ReportBugWebGL: function(message){
    alert(message);
  },
  InputWindow: function(message, defaultValue, targetGameObjectName, targetFunctionName){
    result = window.prompt(message, defaultValue);
    if(result !== "" && result !== null){
      SendMessage(targetGameObjectName, targetFunctionName, result);
    }
  },
});