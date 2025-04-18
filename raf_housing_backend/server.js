import 'dotenv/config';
import express from "express";
import cors from 'cors';

import router from "./authRoutes.js";
import verifyToken from "./authMiddleware.js";
import pool from "./db.js";


import path from "path";
import { fileURLToPath } from "url";
import fs from "fs";
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);


const app = express();
app.use(cors());
app.use(express.json());

app.use("/api/auth", router);

app.get("/api/protected", verifyToken, async (req, res) => {
    try {
        const [rows] = await pool.query("SELECT id, username, email FROM users WHERE id = ?",
            [req.usre.userId]);
        const user = rows[0];
        res.json({ message: "this is hidden data", user });
    } catch (err) {
        console.error(err);
        res.status(500).json({ message: "Internal server error" });
    }
})

const PATH = path.join(__dirname, "events.json");

app.post('/telemetry', (req, res) => {
    try {
        const eventData = req.body;

        let existingEvents = [];
        if (fs.existsSync(PATH)) {
            const rawData = fs.readFileSync(PATH, "utf-8");
            if (rawData.length > 0) {
                existingEvents = JSON.parse(rawData);
            }
        }

        eventData.timestamp = new Date().toISOString();
        existingEvents.push(eventData);

        fs.writeFileSync(PATH, JSON.stringify(existingEvents, null, 2));

        return res.status(200).json({ message: "Data Stored" });
    } catch (error) {
        return res.status(500).json({ error: "Internal Server Error" })
    }
});

const PORT = process.env.Port || 3000;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});