import {InputLabel} from "@components/Inputs";
import { Eye,EyeOff, Mail,Lock } from "lucide-react";
import PasswordInputLabel from "@components/PasswordInputLabel";
import Button from "@components/Buttons/Button";
import Link from "@components/Link";
import {useState} from "react"; 
import { useNavigate } from "react-router-dom";
import { authService } from "../../services/auth/authService";


export default function CartDescription(){

const [email,setEmail]=useState("");
const [password,setPassword]=useState("");
const [error,setError]= useState("");
const navigate = useNavigate();

async function onSubmitHandler(e)
{
    e.preventDefault();
    console.log("onSubmitHandler is clicked");
    const response =await  authService.login ({email:email,password:password});
    if(!response.success){
        setError(response.error);
        setPassword("");
        return;
    }
    setError("");
        navigate("/dashboard");
      //  console.log(`access token ${response.data.token}, refresh token: ${response.data.refreshToken}`);
}

    return (
        <div >        
            <form onSubmit={onSubmitHandler} className="grid grid-cols-1  justify-items-end gap-4 ">
                <InputLabel placeholder="Email" label="Email" type="email" leftIcon={Mail} 
                 onChange={e=>setEmail(e.target.value)} error={error}   />
                <PasswordInputLabel placeholder="Password" label="Password" leftIcon={Lock} rightIcon={Eye}
             rightIconReplacment={EyeOff} onChange={e=>setPassword(e.target.value)} 
                error={error} />

            <Link content="forget password" to="https://preview--swift-inventory-canvas.lovable.app/forgot-password#" />           

            <Button children="Login" type="submit" className="w-[400px]" />
            <Button children="Contact Support" variant="secondary" className="w-[400px]" />
            </form>
            </div>
    )
}

