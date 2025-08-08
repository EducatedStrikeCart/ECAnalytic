import axios from "axios";
import { useState, type ChangeEvent } from "react";

const FileLoader = () => {
    const [uploadedFiles, setUploadedFiles] = useState<FileList | null>(null);
    const [files, setFiles] = useState<File[]>([]);

    const handleSubmit: React.FormEventHandler<HTMLFormElement> = (
        event: React.FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();
        const formData = new FormData();

        // Split large files into smaller files
        if (uploadedFiles && uploadedFiles.length > 0) {
            preprocessFiles(uploadedFiles).forEach((file) => {
                formData.append("file", file);
                // formData.append("fileName", file.name);
            });
        }
        console.log(formData);
        for (const pair of formData.entries()) {
            console.log(pair[0] + ", " + pair[1]);
        }
        axios.post("https://localhost:7170/api/Files", formData, {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        });
    };

    const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files;
        setUploadedFiles(files);
    };

    const preprocessFiles = (files: FileList) => {
        const filesArray: File[] = [];

        if (files) {
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if (file.size > 30_000_000) {
                    splitBigFiles(file).forEach((e: File) =>
                        filesArray.push(e)
                    );
                } else {
                    filesArray.push(file);
                }
            }
        }

        return filesArray;
    };

    const splitBigFiles = (file: File, maxFileSize = 20_000_000): File[] => {
        if (file.size < maxFileSize) return [file];

        const filesArray: File[] = [];
        const chunkSize = maxFileSize;
        const chunksTotal = Math.ceil(file.size / maxFileSize);
        let chunkNumber = 0;

        while (chunkNumber < chunksTotal) {
            const offset = chunkNumber * chunkSize;
            const fileFromBlob = new File(
                [file.slice(offset, chunkSize + offset)],
                "chunk" + chunkNumber
            );
            filesArray.push(fileFromBlob);
            chunkNumber++;
        }

        return filesArray;
    };

    const [test, setTest] = useState<string>("init");
    const handleTest = () => {
        axios.get("https://localhost:7170/api/Files").then((response) => {
            console.log(response.data);
        });
    };

    return (
        <div>
            {test}
            <form onSubmit={handleSubmit}>
                <input type="file" onChange={handleChange} />
                <button type="submit">Upload</button>
            </form>
            <button onClick={handleTest}>TEST</button>
        </div>
    );
};

export { FileLoader };
