import {InputLabel} from "@features/auth/components/Inputs";
import { Eye,EyeOff, Mail,Lock } from "lucide-react";
import PasswordInputLabel from "@features/auth/components/PasswordInputLabel";
import Button from "@components/Buttons/Button";
import Link from "@features/auth/components/Link";
import {useState} from "react"; 
import { useNavigate } from "react-router-dom";
import { authService } from "@shared/services/auth/authService";


export default function CartDescription(){

const [email,setEmail]=useState("");
const [password,setPassword]=useState("");
const [error,setError]= useState("");
const navigate = useNavigate();

async function onSubmitHandler(e)
{
    e.preventDefault();
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

            <Link content="forget password" to="/forgot-password" />

            <Button children="Login" type="submit" className="w-[400px]" />
            <Button children="Contact Support" type="button" variant="secondary" className="w-[400px]" />
            </form>
            </div>
    )
}

