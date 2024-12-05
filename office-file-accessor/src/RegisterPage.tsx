import { useState } from "react";
import { getServerUrl } from "./web/serverUrlGetter";

export function RegisterPage(): JSX.Element {
    const [files, setFiles] = useState<FileList|null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFiles(event.target.files);
  };

  const handleUpload = () => {
    if (!files) {
      alert("Please select files first!");
      return;
    }

    const formData = new FormData();
    Array.from(files).forEach((file) => {
      formData.append("files", file);
    });
    fetch(`${getServerUrl()}/api/files`, {
        method: "POST",
        body: formData,
      })
      .then(res => res.text())
      .then(res => alert(res))
      .catch(err => console.error("Upload failed", err));
  };

  return (
    <div>
        <h1>Register</h1>
      <input type="file" multiple onChange={handleFileChange} />
      <button onClick={handleUpload}>Upload</button>
    </div>
  );
}