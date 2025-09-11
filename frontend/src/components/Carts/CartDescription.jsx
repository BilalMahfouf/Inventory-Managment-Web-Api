import {InputLabel} from "@components/Inputs";
import { Eye,EyeOff, Mail,Lock } from "lucide-react";
import PasswordInputLabel from "@components/PasswordInputLabel";
import Button from "@components/Button";
import Link from "@components/Link";
import {login} from "@services/auth/authService";
import {useState} from "react"; 


export default function CartDescription(){

const [email,setEmail]=useState("");
const [password,setPassword]=useState("");

async function onSubmitHandler(e)
{
    e.preventDefault();
    console.log("onSubmitHandler is clicked");
    const data =await  login ({email:"billelmh501@gmail.com",
        password:"1234"});
        console.log(`access token ${data.token}, refresh token: ${data.refreshToken}`);
}

    return (
        <div >        
            <form onSubmit={onSubmitHandler} className="grid grid-cols-1  justify-items-end gap-4 ">
                <InputLabel placeholder="Email" label="Email" type="email" leftIcon={Mail} 
                 onChange={e=>setEmail(e.target.value)}   />
                <PasswordInputLabel placeholder="Password" label="Password" leftIcon={Lock} rightIcon={Eye}
             rightIconReplacment={EyeOff} onChange={e=>setPassword(e.target.value)} />

            <Link content="forget password" to="https://preview--swift-inventory-canvas.lovable.app/forgot-password#" />           

            <Button children="Login" type="submit" />
            <Button children="Contact Support" variant="secondary" />
            </form>
            </div>
    )
}

