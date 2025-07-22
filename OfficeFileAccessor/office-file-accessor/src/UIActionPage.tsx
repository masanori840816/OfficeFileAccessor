import { useState } from "react";

export function UIActionPage(): JSX.Element {
    const [targetShown, setTargetShown] = useState<Boolean>(false);
    const handleScroll = (event: React.UIEvent<HTMLElement>) => {
        const target = event.currentTarget;

        const scrollY = target.scrollTop;
        setTargetShown(scrollY > 200);
    };
    return <>
        <div className="h-[500px] w-[800px] overflow-y-auto" onScroll={handleScroll}>
            <div className="h-[800px] w-full">
                Scroll me
            </div>
        </div>
        {targetShown ?
            (<div id="target" className="h-[100px] bg-red-500">Target shown</div>):
            (<div>nothing</div>)
        }
    </>;
}