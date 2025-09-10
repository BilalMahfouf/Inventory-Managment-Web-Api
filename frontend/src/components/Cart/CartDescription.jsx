import {InputLabel} from "@components/Inputs";
import { Eye,EyeOff, Mail,Lock } from "lucide-react";
import PasswordInputLabel from "@components/PasswordInputLabel";
import Button from "@components/Button";
import Link from "@components/Link";


export default function CartDescription(){


    return (
        <div >        
            <form className="grid grid-cols-1  justify-items-end gap-4 ">
                <InputLabel placeholder="Email" label="Email" type="email" leftIcon={Mail} />
                <PasswordInputLabel placeholder="Password" label="Password" leftIcon={Lock} rightIcon={Eye}
             rightIconReplacment={EyeOff} />

            <Link content="forget password" to="https://preview--swift-inventory-canvas.lovable.app/forgot-password#" />           

            <Button children="Login"  />
            <Button children="Contact Support" variant="secondary" />
            </form>
            </div>
    )
}