import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import axios from 'axios';
import { useState } from 'react';

function App() {

  const [archivos, setArchivos] = useState(null);

  const subirArchivos = e => {
    setArchivos(e);
  }

  const insertarArchivos = async() => {
    const f = new FormData();

    for (let index = 0; index < archivos.length; index++) {
      f.append("files", archivos[index]);   //files es el nombre del parametro que se usa en la api, debe ser igual
    }

    await axios.post("https://localhost:44319/api/Files", f)
      .then(response => {
        console.log(response.data);
      }).catch(error => {
        console.log(error);
      })
  }

  return (
    <div className="App">
      <input type='file' name='files' multiple onChange={(e) => subirArchivos(e.target.files)}/>
      <button className='btn btn-primary' onClick={insertarArchivos()}>Insertar Archivos</button>
    </div>
  );
}

export default App;
