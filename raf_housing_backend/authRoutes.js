import express from 'express';
const router = express.Router();
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import pool from "./db.js";

router.post("/signup", async (req, res) => {
    try {
        const { username, email, password } = req.body;

        if (!username || !email || !password) {
            return res.status(400).json({ message: "Missing username/email/password" });
        };

        const [existing] = await pool.query(
            "SELECT id FROM users WHERE username = ? OR email = ?",
            [username, email]
        );
        if (existing.length > 0) {
            return res.status(400).json({ message: "User or email already exists" })
        };

        const saltRounds = 10;
        const hashedPassword = await bcrypt.hash(password, saltRounds);

        await pool.query(
            "INSERT INTO users (username, email, password_hash) VALUES (?,?,?)",
            [username, email, hashedPassword]
        );

        res.status(201).json({ message: "User created succesfrly" });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: "Internal server error" });
    }
});

router.post("/signin", async (req, res) => {
    try {
        const { username, email, password } = req.body;

        if (!username || !password) {
            return res.status(400).json({ message: "Missing username/password" });
        };

        const [existing] = await pool.query(
            "SELECT id, username, password_hash FROM users WHERE username = ?",
            [username]
        );

        if (existing.length === 0) {
            return res.status(401).json({ message: "Invalid credentials" });
        }

        const user = existing[0];

        const match = await bcrypt.compare(password, user.password_hash)
        if (!match) {
            return res.status(401).json({ message: "Invalid credentials" });
        }

        // will be using
        const token = jwt.sign( 
            { userId: user.id, username: user.username },
            process.env.JWT_SECRET,
            { expiresIn: "1d" }
        );

        return res.status(200).json({ message: "Logged in", token });
    } catch (error) {
        console.error(error)
        return res.status(500).json({ message: "Internal server error" });
    }
});

export default router;