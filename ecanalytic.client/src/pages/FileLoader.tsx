import axios from "axios";
import { useState, type ChangeEvent } from "react";

const FileLoader = () => {
    const [file, setFile] = useState<File>();

    const handleSubmit = () => {
        const formData = new FormData();
        if (file) {
            formData.append("file", file);
            formData.append("fileName", file.name);
            axios.post("/Files", formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            });
        }
    };

    const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files;
        if (files) {
            const selectedFile = files[0];
            setFile(selectedFile);
        }
    };

    return (
        <div>
            <form>
                <input type="file" onChange={handleChange} />
                <button type="submit" onSubmit={handleSubmit}>
                    Upload
                </button>
            </form>
           {file?.name}
        </div>
    );
};

export { FileLoader };
